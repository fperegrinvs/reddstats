using System.Text;
using System.Collections.Generic;

namespace bsparser
{
    using System;
    using System.IO;
    using System.Linq;

    using CryptSharp.Utility;

    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class BlockParser
    {
        public const long End = 0xdbb6c0fb;

        public const decimal Divide = 100000000M;

        public const string BlockPath = "E:\\Reddcoin\\Blockchain\\blocks\\blk$n$.dat";

        protected string CurrentFile { get; set; }

        protected List<Block> Blocks = new List<Block>();

        protected Dictionary<string, ulong> Balances { get; set; }

        protected Dictionary<string, ulong> NonzeroBalances { get; set; }

        /// <summary>
        /// Source - Target
        /// </summary>
        protected Dictionary<string, string> LinkedAccounts { get; set; }

        public Dictionary<string, HashSet<string>> LinkedAccountList { get; set; }

        protected List<Summary> History { get; set; }

        protected Dictionary<string, Tuple<string, ulong>> Transactions { get; set; }

        protected decimal TotalMoney { get; set; }

        protected const int SnapInterval = 60 * 24 * 7;

        public void ParseChain()
        {
            int i = 0;

            while (this.ParseFile(i++))
            {
            }
        }

        public void SaveFile(string filename)
        {
            using (var file = File.Open(filename, FileMode.OpenOrCreate))
            {
                var ef = new ExcelPackage(file);
                for (var i = 0; i < History.Count; i += 2)
                {
                    var excel = ef.Workbook.Worksheets.Add("Week " + ((i / 2) + 1));

                    excel.Cells["A1:C1"].Merge = true;
                    excel.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells["A11:C11"].Merge = true;
                    excel.Cells["A11:C11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells[1, 1].Value = string.Format("Week {0} (Blocks {1} - {2})", (i / 2) + 1, History[i].StartBlock, History[i].EndBlock);

                    excel.Cells["F1:H1"].Merge = true;
                    excel.Cells["F1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells["F11:H11"].Merge = true;
                    excel.Cells["F11:H11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    excel.Cells[1, 6].Value = string.Format("Week {0} (Total)", ((i / 2) + 1));

                    WriteSummary(excel, 2, History[i]);
                    WriteSummary(excel, 7, History[i + 1]);
                }

                ef.Save();
            }
        }

        private void WriteSummary(ExcelWorksheet excel, int left, Summary history)
        {
            for (var m = 2; m < 11; m++)
            {
                excel.Cells[m, left - 1, m, left].Merge = true;
            }

            excel.Cells[2, left - 1].Value = "Total Redds";
            excel.Cells[2, left + 1].Value = history.TotalMoney;
            excel.Cells[2, left + 1].Style.Numberformat.Format = "#,##0.00;-#,##0.00";

            excel.Cells[3, left - 1].Value = "# Accounts";
            excel.Cells[3, left + 1].Value = history.TotalAddresses;
            excel.Cells[3, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[4, left - 1].Value = "# Active Accounts";
            excel.Cells[4, left + 1].Value = history.TotalNonZeroAddresses;
            excel.Cells[4, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[5, left - 1].Value = "Transactions";
            excel.Cells[5, left + 1].Value = history.TotalTransactions;
            excel.Cells[5, left + 1].Style.Numberformat.Format = "#,##0";

            excel.Cells[6, left - 1].Value = "Transactions/Block";
            excel.Cells[6, left + 1].Value = history.TotalTransactions / (history.EndBlock - history.StartBlock + 1M);
            excel.Cells[6, left + 1].Style.Numberformat.Format = "#,##0.00";

            excel.Cells[7, left - 1].Value = "Top 10 (%)";
            excel.Cells[7, left + 1].Value = string.Format("{0} ({1}%)", history.Top10Total.ToString("N0"), (history.Top10Total / history.TotalMoney * 100).ToString("N2"));
            excel.Cells[7, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            excel.Cells[8, left - 1].Value = "Top 100 (%)";
            excel.Cells[8, left + 1].Value = string.Format("{0} ({1}%)", history.Top100Total.ToString("N0"), (history.Top100Total / history.TotalMoney * 100).ToString("N2"));
            excel.Cells[8, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            excel.Cells[9, left - 1].Value = "Theil Index";
            if (history.TheilIndex > 0)
            {
                excel.Cells[9, left + 1].Value = history.TheilIndex;
                excel.Cells[9, left + 1].Style.Numberformat.Format = "#,##0.00";
            }
            else
            {
                excel.Cells[9, left + 1].Value = "N/A";
                excel.Cells[9, left + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            excel.Cells[10, left - 1].Value = "Gini Index";
            excel.Cells[10, left + 1].Value = history.GniIndex;
            excel.Cells[10, left + 1].Style.Numberformat.Format = "#,##0.00";

            excel.Cells[11, left - 1].Value = "Accounts";
            excel.Cells[12, left - 1].Value = "Rank";
            excel.Cells[12, left].Value = "Address";
            excel.Cells[12, left + 1].Value = "Balance";

            var index = 0;
            for (var a = 0; a < history.TopAccounts.Count; a++)
            {
                excel.Cells[13 + index, left].Value = history.TopAccounts[a].Address;
                if (!LinkedAccountList.ContainsKey(history.TopAccounts[a].Address))
                {
                    excel.Cells[13 + index, left + 1].Value = history.TopAccounts[a].Balance;
                    excel.Cells[13 + index, left - 1].Value = a + 1;
                }
                else
                {
                    var begin = index;
                    foreach (var secondary in LinkedAccountList[history.TopAccounts[a].Address])
                    {
                        // do not list insignificant addresses
                        if (Balances[secondary] >= 100 * Divide)
                        {
                            index++;
                            excel.Cells[13 + index, left].Value = secondary;
                        }
                    }

                    excel.Cells[13 + begin, left + 1, 13 + index, left + 1].Merge = true;
                    excel.Cells[13 + begin, left + 1].Value = history.TopAccounts[a].Balance;

                    excel.Cells[13 + begin, left - 1, 13 + index, left - 1].Merge = true;
                    excel.Cells[13 + begin, left - 1].Value = a + 1;
                }

                index++;
            }


            excel.Cells[13, left - 1, 12 + index, left - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            excel.Cells[13, left + 1, 12 + index, left + 1].Style.Numberformat.Format = "#,##0";
            excel.Cells[13, left + 1, 12 + index, left + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            excel.Column(left).AutoFit();
            excel.Column(left + 1).AutoFit();
        }

        public void ProcessBalances(bool linkAccount = false)
        {
            Transactions = new Dictionary<string, Tuple<string, ulong>>();
            Balances = new Dictionary<string, ulong>();
            NonzeroBalances = new Dictionary<string, ulong>();
            History = new List<Summary>();
            LinkedAccounts = new Dictionary<string, string>();
            LinkedAccountList = new Dictionary<string, HashSet<string>>();

            var i = 0;
            foreach (var block in Blocks)
            {
                i++;

                var t = 0;
                foreach (var trans in block.Transactions)
                {
                    foreach (TxInput input in trans.Inputs)
                    {
                        if (t == 0) // generation
                        {
                            continue;
                        }

                        if (input.PreviousTxOutputKey.TxHash > new UInt256(0))
                        {
                            var key = input.PreviousTxOutputKey.TxHash + "#" + input.PreviousTxOutputKey.TxOutputIndex;
                            var debitAccount = Transactions[key];

                            // transaction with multiple inputs. Should I link the accounts or this is already a master account ?
                            if (linkAccount && trans.Inputs.Count > 1 && !LinkedAccountList.ContainsKey(debitAccount.Item1))
                            {
                                string mainAccountAddress;

                                if (LinkedAccounts.ContainsKey(debitAccount.Item1))
                                {
                                    mainAccountAddress = LinkedAccounts[debitAccount.Item1];
                                }
                                else
                                {
                                    var mainAccountTransactionCandidate = trans.Inputs[0].PreviousTxOutputKey.TxHash + "#" + trans.Inputs[0].PreviousTxOutputKey.TxOutputIndex;
                                    var mainAccountAddressCandidate = Transactions[mainAccountTransactionCandidate].Item1;

                                    // if main account candidate point to other account, the pointed account is the real main
                                    mainAccountAddress = LinkedAccounts.ContainsKey(mainAccountAddressCandidate)
                                        ? LinkedAccounts[mainAccountAddressCandidate]
                                        : mainAccountAddressCandidate;
                                }

                                // check if this is the main account
                                if (mainAccountAddress != debitAccount.Item1)
                                {
                                    LinkedAccounts[debitAccount.Item1] = mainAccountAddress;

                                    if (!LinkedAccountList.ContainsKey(mainAccountAddress))
                                    {
                                        LinkedAccountList[mainAccountAddress] = new HashSet<string>();
                                    }

                                    LinkedAccountList[mainAccountAddress].Add(debitAccount.Item1);
                                }
                            }

                            var balance = Balances[debitAccount.Item1] -= debitAccount.Item2;
                            this.TotalMoney -= debitAccount.Item2 / Divide;

                            if (balance == 0)
                            {
                                NonzeroBalances.Remove(debitAccount.Item1);
                            }
                        }
                    }

                    var o = 0;
                    foreach (var output in trans.Outputs)
                    {
                        if (output.Value == 0 && t == 0 && o == trans.Outputs.Count - 1)
                        {
                            continue;
                        }

                        var d = DataCalculator.GetToAddress(output.ScriptPublicKey.ToArray());

                        this.TotalMoney += t != 0  ? output.Value / Divide : output.Value;

                        if (Balances.ContainsKey(d))
                        {
                            var balance = Balances[d] += output.Value;
                            NonzeroBalances[d] = balance;
                        }
                        else
                        {
                            Balances[d] = output.Value;
                            NonzeroBalances[d] = output.Value;
                        }

                        Transactions[trans.Hash + "#" + o] = new Tuple<string, ulong>(d, output.Value);

                        o++;
                    }

                    t++;
                }

                if (i % SnapInterval == 0 || i == this.Blocks.Count - 1)
                {
                    var diff = this.CreateSummary(History.Count > 0, i, linkAccount);
                    var total = History.Count > 0 ? this.CreateSummary(false, i, linkAccount) : diff;
                    History.Add(diff);
                    History.Add(total);
                }
            }
        }

        public Summary CreateSummary(bool isDiff, int position, bool linkAccounts)
        {
            var pastSummary = isDiff ? History.Last() : null;
            var summary = new Summary();
            summary.StartBlock = isDiff ? pastSummary.EndBlock + 1 : 0;
            summary.EndBlock = position;
            summary.NonZero = new Dictionary<string, decimal>(NonzeroBalances.Count);
            summary.TotalMoney = isDiff ? this.TotalMoney - pastSummary.TotalMoney : this.TotalMoney;

            foreach (var balance in this.Balances)
            {
                // skup secundary linked accounts
                if (linkAccounts && LinkedAccounts.ContainsKey(balance.Key))
                {
                    continue;
                }

                Decimal past = 0;
                if (isDiff)
                {
                    for (var h = History.Count - 1; h >= 0; h--)
                    {
                        if (History[h].NonZero.ContainsKey(balance.Key))
                        {
                            past = History[h].NonZero[balance.Key];
                        }
                    }
                }

                var balanceValue = balance.Value;

                // this is a main account
                if (linkAccounts && LinkedAccountList.ContainsKey(balance.Key))
                {
                    balanceValue = this.LinkedAccountList[balance.Key].Aggregate(balanceValue, (current, secundary) => current + this.Balances[secundary]);
                }

                var delta = (balanceValue / Divide) - past;
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


            summary.TotalAddresses = (ulong)this.Balances.Count;
            summary.TotalTransactions = isDiff ? this.Transactions.Count - pastSummary.TotalTransactions : this.Transactions.Count;
            summary.Top10Total = summary.TopAccounts.Take(10).Sum(x => x.Balance);
            summary.Top100Total = summary.TopAccounts.Take(100).Sum(x => x.Balance);
            return summary;
        }

        private bool ParseFile(int order)
        {
            CurrentFile = BlockPath.Replace("$n$", order.ToString("D5"));
            if (!File.Exists(CurrentFile))
            {
                return false;
            }

            var stream = new FileStream(CurrentFile, FileMode.Open, FileAccess.Read);


            Block block;
            do
            {
                block = ReadBlock(stream);

                if (block != null)
                {
                    if (Blocks.Count > 0)
                    {
                        Blocks.Last().Hash = block.PreviousBlockHash;
                    }

                    Blocks.Add(block);
                }
            }
            while (block != null);

            return true;
        }

        private static Block ReadBlock(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                if (reader.PeekChar() == -1)
                {
                    return null;
                }

                var block = DecodeBlockHeader(stream);

                if (block == null)
                {
                    return null;
                }

                var transactions = reader.DecodeList(() => DecodeTransaction(stream));

                block.Transactions = transactions;

                return block;
            }
        }

        private static Block DecodeBlockHeader(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                reader.Read8Bytes(); // 4 bytes magic, 4 bytes size

                var headerBin = reader.ReadBytes(80);

                //var blockHash = SCrypt.ComputeDerivedKey(headerBin, headerBin, 1024, 1, 1, 1, 32).ToHexStringReverse();

                var version = BitConverter.ToUInt32(headerBin, 0); // reader.Read4Bytes();

                if (version == 0)
                {
                    return null;
                }

                var previousBlock = headerBin.ToHexStringReverse(4, 32); //reader.Read32BytesString();
                var merkleRoot = headerBin.ToHexStringReverse(36, 32);// reader.Read32BytesString();
                var time = BitConverter.ToUInt32(headerBin, 68).ToDateTime(); // reader.Read4Bytes().ToDateTime();
                var bits = BitConverter.ToUInt32(headerBin, 72).GetDifficulty();// reader.Read4Bytes().GetDifficulty();
                var nonce = BitConverter.ToUInt32(headerBin, 76);// reader.Read4Bytes();
                return new Block(version, previousBlock, merkleRoot, time, bits, nonce);
            }
        }

        private static TxInput DecodeTxInput(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                return new TxInput
                (
                    new TxOutputKey(reader.Read32Bytes(),
                        reader.Read4Bytes()),
                        reader.ReadVarBytes().ToList(),
                        reader.Read4Bytes()
                );
            }
        }

        private static TxOutput DecodeTxOutput(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                return new TxOutput
                (
                    reader.Read8Bytes(),
                    reader.ReadVarBytes().ToList()
                );
            }
        }

        private static Transaction DecodeTransaction(Stream stream, UInt256? txHash = null)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                return new Transaction(
                    reader.Read4Bytes(),
                    reader.DecodeList(() => DecodeTxInput(stream)),
                    reader.DecodeList(() => DecodeTxOutput(stream)),
                    reader.Read4Bytes(),
                    txHash);
            }
        }
    }
}

