﻿using HomeBankingMindHub.Models;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.dtos

{
    public class AccountDTO
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime CreationDate { get; set; }

        public double Balance { get; set; }

        public ICollection<TransactionDTO> Transactions { get; set; }
        public AccountDTO() { }
        public AccountDTO(Account account)
        {
            Id = account.Id;
            Number = account.Number;
            CreationDate = account.CreationDate;
            Balance = account.Balance;
            Transactions = account.Transactions.IsNullOrEmpty()? null : account.Transactions.Select(transaction => new TransactionDTO(transaction)).ToList();
      
        }
        public AccountDTO(Client client)
        {
            List<AccountDTO> accounts = client.Accounts.Select(account => new AccountDTO
            {
                Id = account.Id,
                Number = account.Number,
                CreationDate = account.CreationDate,
                Balance = account.Balance,
            }).ToList();
        }

    }

}