"use strict"; // http://stackoverflow.com/questions/1335851/what-does-use-strict-do-in-javascript-and-what-is-the-reasoning-behind-it

angular.module("reddstats.services")
.service("BlockChainService", function () {
    return {
        GetAddress: function (address, callback) {
            callback(
            {
                Address: "Ra6DiiqLyHUKG4U2KQfnYfZjCCiXdburwD",
                Transactions: [{ Id: "blala", Block: 2, Date: "10/10/2013", Ammount: 2020, Balance: 500 },
                    { Id: "outro", Block: 3, Date: "10/10/2013", Ammount: 1024, Balance: 1500 }]
            });
        }
    };
});