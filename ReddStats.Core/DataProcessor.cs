namespace ReddStats.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ReddStats.Core.VO;

    public static class DataProcessor
    {
        public static ConsolidatedData ConsolidateData(BlockChain chain, ConsolidatedData previousConsolidatedData = null, int startBlock = 0, int? endBlock = null)
        {
            if (!endBlock.HasValue)
            {
                endBlock = chain.Blocks.Count;
            }

            var consolidated = new ConsolidatedData
                               {
                                   RelatedAccounts = previousConsolidatedData != null ? previousConsolidatedData.RelatedAccounts : new RelatedAccounts(),
                                   AccountBalances = new Dictionary<string, decimal>(),
                                   Accounts = new Dictionary<string, Account>(),
                                   EndBlock = endBlock.Value,
                                   TotalMoney = previousConsolidatedData != null ? previousConsolidatedData.TotalMoney : 0M,
                                   TotalTransactions = previousConsolidatedData != null ? previousConsolidatedData.TotalTransactions : 0,
                                   TotalTransactionsValue = previousConsolidatedData != null ? previousConsolidatedData.TotalTransactionsValue : 0M
                               };

            var blocks = chain.Blocks.Skip(startBlock).Take(endBlock.Value - startBlock);

            foreach (var block in blocks)
            {
                consolidated.TotalTransactions += block.TransactionsCount;

                var t = 0;
                foreach (var trans in block.Transactions)
                {
                    foreach (TransactionInput input in trans.Inputs)
                    {
                        if (t == 0) // generation
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(input.PreviousOutputKey))
                        {
                            // linked accounts
                            if (trans.Inputs.Count > 1)
                            {
                                var mainAccount = consolidated.RelatedAccounts.GetMainAccount(trans.Inputs[1].FromAddress);
                                consolidated.RelatedAccounts.AddRelatedAccount(mainAccount, input.FromAddress);

                                if (!consolidated.Accounts.ContainsKey(mainAccount))
                                {
                                    consolidated.Accounts[mainAccount] = new Account();
                                }

                                consolidated.Accounts[mainAccount].RelatedAccounts = consolidated.RelatedAccounts.LinkedAccountList[mainAccount];
                            }

                            if (consolidated.AccountBalances.ContainsKey(input.FromAddress))
                            {
                                consolidated.AccountBalances[input.FromAddress] -= input.Amount;
                            }
                            else
                            {
                                consolidated.AccountBalances[input.FromAddress] = input.Amount;
                                consolidated.Accounts[input.FromAddress] = new Account();
                            }

                            consolidated.TotalMoney -= input.Amount;
                            consolidated.Accounts[input.FromAddress].Transactions.Add(new AccountTransaction
                                                                                      {
                                                                                          BlockId = block.Id,
                                                                                          TransactionId = trans.TransactionId,
                                                                                          Date = block.Date,
                                                                                          TransactionType = TransactionTypeEnum.Output,
                                                                                          TransactionIndex = input.Index,
                                                                                          Value = input.Amount
                                                                                      });
                        }
                    }

                    var o = 0;
                    foreach (var output in trans.Outputs)
                    {
                        if (output.Amount == 0 && t == 0 && o == trans.Outputs.Count - 1)
                        {
                            continue;
                        }

                        if (consolidated.AccountBalances.ContainsKey(output.ToAddress))
                        {
                            consolidated.AccountBalances[output.ToAddress] += output.Amount;
                        }
                        else
                        {
                            consolidated.AccountBalances[output.ToAddress] = output.Amount;
                            consolidated.Accounts[output.ToAddress] = new Account();
                        }

                        consolidated.TotalMoney += output.Amount;
                        consolidated.TotalTransactionsValue += output.Amount;

                        consolidated.Accounts[output.ToAddress].Transactions.Add(new AccountTransaction
                        {
                            BlockId = block.Id,
                            TransactionId = trans.TransactionId,
                            Date = block.Date,
                            TransactionType = TransactionTypeEnum.Output,
                            TransactionIndex = output.Index,
                            Value = output.Amount
                        });

                        o++;
                    }

                    t++;
                }
            }

            return consolidated;
        }

        public static Summary CreateSummary(ConsolidatedData data, bool linkAccounts = false, ConsolidatedData previous = null)
        {
            var summary = new Summary();
            summary.StartBlock = previous != null ? previous.EndBlock + 1 : 0;
            summary.EndBlock = data.EndBlock;
            summary.NonZero = new Dictionary<string, decimal>();
            summary.TotalMoney = previous != null ? data.TotalMoney - previous.TotalMoney : data.TotalMoney;

            foreach (var balance in data.AccountBalances)
            {
                // skup secundary linked accounts
                if (linkAccounts && data.RelatedAccounts.LinkedAccounts.ContainsKey(balance.Key))
                {
                    continue;
                }

                Decimal past = 0;
                if (previous != null)
                {
                    if (previous.AccountBalances.ContainsKey(balance.Key))
                    {
                        past = previous.AccountBalances[balance.Key];
                    }
                }

                var balanceValue = balance.Value;

                // this is a main account
                if (linkAccounts && data.RelatedAccounts.GetMainAccount(balance.Key) == balance.Key)
                {
                    balanceValue = data.RelatedAccounts.LinkedAccountList[balance.Key].Aggregate(balanceValue, (current, secundary) => current + data.AccountBalances[secundary]);
                }

                var delta = (balanceValue) - past;
                if (Math.Abs(delta) > 100)
                {
                    summary.NonZero[balance.Key] = delta;
                }
            }

            summary.TopAccounts = (from item in summary.NonZero
                                   orderby item.Value descending
                                   select new SimpleAccount { Address = item.Key, Balance = item.Value }).ToList();

            summary.TotalNonZeroAddresses = summary.TopAccounts.Count;


            var theil = 0M;
            var gini = 0M;

            var haveTheil = summary.TopAccounts.Last().Balance >= 0;

            var n = 0M;
            foreach (var balance in summary.TopAccounts)
            {
                n++;
                var b = balance.Balance;

                var dev = b / summary.Average;
                gini += n * b;
                if (haveTheil)
                {
                    theil += dev * (decimal)Math.Log((double)dev);
                }
            }

            summary.TheilIndex = haveTheil ? theil / (n * (decimal)Math.Log((double)n)) : 0;
            summary.GniIndex = ((n + 1) / (n - 1)) - (2 * gini / (n * (n - 1) * summary.Average));

            summary.TotalAddresses = (ulong)data.AccountBalances.Count;
            summary.TotalTransactions = previous != null ? data.TotalTransactions - previous.TotalTransactions : data.TotalTransactions;
            summary.Top10Total = summary.TopAccounts.Take(10).Sum(x => x.Balance);
            summary.Top100Total = summary.TopAccounts.Take(100).Sum(x => x.Balance);
            return summary;
        }


    }
}
