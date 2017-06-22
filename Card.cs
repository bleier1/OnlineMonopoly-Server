// The Card class, which represents the Chance and Community Chest cards drawn from the decks on the board.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Card : Object
    {
        // Default constructor.
        public Card()
        {
            m_description = "N/A";
            m_action = "N/A";
        }
        // Constructor that allows for initialization of the description and the action.
        public Card(string a_description, string a_action)
        {
            m_description = a_description;
            m_action = a_action;
        }
        // Properties for the Card's description.
        public string Description
        {
            get
            {
                return m_description;
            }
        }
        // Properties for the Card's action.
        public string Action
        {
            get
            {
                return m_action;
            }
            set
            {
                m_action = value;
            }
        }


        // The description of the card (what it says).
        private string m_description;
        // The "action" of the card (what action must be performed upon drawing the card).
        private string m_action;
    }
}