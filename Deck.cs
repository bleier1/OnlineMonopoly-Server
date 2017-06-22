// The Deck class, which represents a deck of Chance or Community Chest cards to draw from on the board.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Deck : Object
    {
        // Default constructor that just initializes the ArrayList with nothing in it.
        public Deck()
        {
            m_deck = new ArrayList();
        }
        // Constructor that initializes a deck of cards specified by the string passed in.
        public Deck(string a_deckName)
        {
            // Only initialize the deck if the name is either "chance" or "community."
            if (a_deckName == "chance")
            {
                // Add the Chance cards to the deck.
                m_deck = new ArrayList();
                m_deck.Add(new Card("Advance to Go (Collect $200)", "advanceGo"));
                m_deck.Add(new Card("Advance to Illinois Ave.", "advanceIllinois"));
                m_deck.Add(new Card("Advance to St. Charles Place. If you pass Go, collect $200", "advanceStCharles"));
                m_deck.Add(new Card("Advance to nearest Utility", "advanceUtility"));
                m_deck.Add(new Card("Advance to nearest Railroad", "advanceRailroad"));
                m_deck.Add(new Card("Advance to nearest Railroad", "advanceRailroad"));
                m_deck.Add(new Card("Bank pays you dividend of $50", "bankDividend"));
                m_deck.Add(new Card("Get Out of Jail Free!", "jailFree"));
                m_deck.Add(new Card("Go back 3 spaces", "goBack3"));
                m_deck.Add(new Card("Go directly to Jail. Do not pass Go, do not collect $200", "jail"));
                m_deck.Add(new Card("Make general repairs on all your property. For each house, pay $25. For each hotel, pay $100.", "generalRepairs"));
                m_deck.Add(new Card("Pay Poor Tax of $15", "poorTax"));
                m_deck.Add(new Card("Take a ride on the Reading. If you pass Go, collect $200", "readingRailroad"));
                m_deck.Add(new Card("Take a walk on the Boardwalk", "boardwalk"));
                m_deck.Add(new Card("You have been elected Chairman of the Board. Pay each player $50", "chairman"));
                m_deck.Add(new Card("Your building and loan matures. Collect $150", "matures"));
            }
            else if (a_deckName == "community")
            {
                // Add the Community Chest cards to the deck.
                m_deck = new ArrayList();
                m_deck.Add(new Card("Advance to Go (Collect $200)", "advanceGo"));
                m_deck.Add(new Card("Bank error in your favor. Collect $200", "bankError"));
                m_deck.Add(new Card("Doctor's fee. Pay $50", "doctorsFee"));
                m_deck.Add(new Card("From sale of stock you get $50", "stockSale"));
                m_deck.Add(new Card("Get Out of Jail Free!", "jailFree"));
                m_deck.Add(new Card("Go directly to Jail. Do not pass Go, do not collect $200", "jail"));
                m_deck.Add(new Card("Grand Opera opening. Collect $50 from each player for opening night seats", "grandOpera"));
                m_deck.Add(new Card("Xmas fund matures. Collect $100", "xmasFund"));
                m_deck.Add(new Card("Income tax refund. Collect $20", "taxRefund"));
                m_deck.Add(new Card("Life insurance matures. Collect $100", "lifeInsurance"));
                m_deck.Add(new Card("Pay hospital $100", "hospitalFees"));
                m_deck.Add(new Card("Pay School Tax of $150", "schoolTax"));
                m_deck.Add(new Card("Receive for services $25", "services"));
                m_deck.Add(new Card("You are assessed for street repairs. $40 per house, $115 per hotel.", "streetRepairs"));
                m_deck.Add(new Card("You have won second prize in a beauty contest. Collect $10", "beautyContest"));
                m_deck.Add(new Card("You inherit $100", "inherit100"));
            }
            else
            {
                // Do nothing, this isn't a real name for the deck.
                m_deck = new ArrayList();
            }
        }

        /**/
        /*
        ShuffleDeck()

        NAME

                ShuffleDeck() - shuffles the deck

        SYNOPSIS

                public void ShuffleDeck()

        DESCRIPTION

                This function "shuffles" the deck. This is achieved through initializing a
                random seed, then going through a for loop 25 times. In each iteration of the
                loop, it picks a random card in the deck to "take out" and "put at the bottom,"
                achieved through assigning a Card object to the random index, removing the card
                at said index, then adding it to the ArrayList.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                8:37pm 3/5/2017

        */
        /**/
        public void ShuffleDeck()
        {
            // Initialize a Random seed.
            Random rand = new Random();
            // 25 times seems enough for a good shuffle.
            for (int i = 0; i < 25; i++)
            {
                // Choose a random index in the deck.
                int randIndex = rand.Next(0, 16);
                // Take that card out of the deck, then put it back in at the bottom.
                Card randCard = (Card)m_deck[randIndex];
                m_deck.RemoveAt(randIndex);
                m_deck.Add(randCard);
            }
        }
        // Gets the card on the top of the deck.
        public Card GetCardOnTop()
        {
            return (Card)m_deck[0];
        }
        // Moves the card from the top of the deck to the bottom.
        public void MoveTopCardToBottom()
        {
            Card cardToMove = this.GetCardOnTop();
            // Remove the card in the first position of the ArrayList, then add it back to the end of it.
            m_deck.RemoveAt(0);
            m_deck.Add(cardToMove);
        }

        // An ArrayList that contains the cards. This is what will be the "deck" to draw from.
        private ArrayList m_deck;
    }
}