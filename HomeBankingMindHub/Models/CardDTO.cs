﻿using HomeBankingMindHub.Models;
using System;

namespace HomeBankingMindHub.dtos
{
    public class CardDTO
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }
        public CardDTO() { }
        public CardDTO(Card card)
        {
            Id = card.Id;
            CardHolder = card.CardHolder;
            Color = card.Color.ToString();
            Cvv = card.Cvv;
            FromDate = card.FromDate;
            Number = card.Number;
            ThruDate = card.ThruDate;
            Type = card.Type.ToString();
            
        }
        public CardDTO(Client client)
        {
            List<CardDTO> cards = client.Cards.Select(card => new CardDTO
            {
                Id = card.Id,
                CardHolder = card.CardHolder,
                Type = card.Type.ToString(),
                Color = card.Color.ToString(),
                Number = card.Number,
                Cvv = card.Cvv,
                FromDate = card.FromDate,
                ThruDate = card.ThruDate,
            }).ToList();
        }

    }
}
