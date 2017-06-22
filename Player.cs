// The Player class, which represents the players that are playing the game.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineMonopoly
{
    class Player : Object
    {
        // Default constructor.
        public Player()
        {
            m_name = "N/A";
            m_funds = 0;
            m_token = "N/A";
            m_properties = new ArrayList();
            m_getOutOfJailAmount = 0;
            m_jailTurnCount = 0;
            m_doublesCount = 0;
            m_boardPosition = 0;
            m_inJail = false;
            m_houseAmount = 0;
            m_hotelAmount = 0;
            m_dieRoll = 0;
            m_rosterPosition = 0;
            m_passedGo = false;
        }
        // Constructor that defines the player's name and their token.
        public Player(string a_name, string a_token)
        {
            m_name = a_name;
            // Players always start with $1500.
            m_funds = 1500;
            m_token = a_token;
            m_properties = new ArrayList();
            m_getOutOfJailAmount = 0;
            m_jailTurnCount = 0;
            m_doublesCount = 0;
            m_boardPosition = 0;
            m_inJail = false;
            m_houseAmount = 0;
            m_hotelAmount = 0;
            m_dieRoll = 0;
            m_rosterPosition = 0;
            m_passedGo = false;
        }

        // Properties for the player's funds.
        public int Funds
        {
            get
            {
                return m_funds;
            }
            set
            {
                m_funds = value;
            }
        }
        // Properties to get and set the player's position on the board.
        public int Position
        {
            get
            {
                return m_boardPosition;
            }
            set
            {
                m_boardPosition = value;
            }
        }
        // Property to get the player's double count.
        public int DoublesCount
        {
            get
            {
                return m_doublesCount;
            }
            set
            {
                m_doublesCount = value;
            }
        }
        // Properties to get the player's Get Out of Jail Free cards.
        public int GetOutOfJailAmount
        {
            get
            {
                return m_getOutOfJailAmount;
            }
            set
            {
                m_getOutOfJailAmount = value;
            }
        }
        // Properties to determine the status of the player being in jail.
        public bool IsInJail
        {
            get
            {
                return m_inJail;
            }
            set
            {
                m_inJail = value;
            }
        }
        // Properties to determine the number of turns the player has been in jail for.
        public int JailTurnCount
        {
            get
            {
                return m_jailTurnCount;
            }
            set
            {
                m_jailTurnCount = value;
            }
        }
        // Properties to get the player's name.
        public string Name
        {
            get
            {
                return m_name;
            }
        }
        // Properties to get the list of properties the player has.
        public ArrayList Properties
        {
            get
            {
                return m_properties;
            }
        }
        // Properties to get and set the amount of houses a player has.
        public int HouseAmount
        {
            get
            {
                return m_houseAmount;
            }
            set
            {
                m_houseAmount = value;
            }
        }
        // Properties to get and set the amount of hotels a player has.
        public int HotelAmount
        {
            get
            {
                return m_hotelAmount;
            }
            set
            {
                m_hotelAmount = value;
            }
        }
        // Properties to get and set the player's die roll.
        public int DieRoll
        {
            get
            {
                return m_dieRoll;
            }
            set
            {
                m_dieRoll = value;
            }
        }
        // Propeties to get and set the player's roster position.
        public int RosterPosition
        {
            get
            {
                return m_rosterPosition;
            }
            set
            {
                m_rosterPosition = value;
            }
        }
        // Properties to get and set the player's status of whether or not they passed Go.
        public bool PassedGo
        {
            get
            {
                return m_passedGo;
            }
            set
            {
                m_passedGo = value;
            }
        }

        /**/
        /*
        BuyProperty()

        NAME

                BuyProperty() - buys a property

        SYNOPSIS

                public void BuyProperty(ref Property a_property)

                a_property -> the property to buy on the board

        DESCRIPTION

                This function lets the player "buy" a property on the board. It subtracts the price
                of the property from the player's funds, adds it to the ArrayList of Properties, then
                changes the property's owner to match the name of the player.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                7:47pm 3/13/2017

        */
        /**/
        public void BuyProperty(ref Property a_property)
        {
            // Buy the property, add it to the player's ArrayList of properties, and change
            // its owner.
            m_funds -= a_property.Price;
            a_property.Owner = this;
            m_properties.Add(a_property);
        }
        /**/
        /*
        RollDice()

        NAME

                RollDice() - rolls two pairs of dice

        SYNOPSIS

                public void RollDice(ref int a_firstDie, ref int a_secondDie)

                a_firstDie -> the first die to be rolled
                a_secondDIe -> the second die to be rolled

        DESCRIPTION

                This function lets the player roll a pair of dice to make a move on the board.
                Random seeds are generated to get numbers between 1-6. If the function detects
                they are "doubles," then the player's double count is incremented.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                9:58pm 3/13/2017

        */
        /**/
        public void RollDice(ref int a_firstDie, ref int a_secondDie)
        {
            Random rand = new Random();
            a_firstDie = rand.Next(1, 7);
            a_secondDie = rand.Next(1, 7);
            // Record the die roll and display it to the console.
            m_dieRoll = a_firstDie + a_secondDie;
            Console.WriteLine(m_name + " rolled a " + m_dieRoll.ToString() + ".");
            // Are the rolls the same? If so, increment the doubles counter.
            if (a_firstDie == a_secondDie)
            {
                m_doublesCount++;
                Console.WriteLine("Doubles rolled: " + m_doublesCount.ToString());
            }
            // Otherwise, put it at 0.
            else m_doublesCount = 0;
        }
        /**/
        /*
        MakeMove()

        NAME

                MakeMove() - makes the player move on the board

        SYNOPSIS

                public void MakeMove(int a_spacesToMove)

                a_spacesToMove -> the amount of spaces the player will move

        DESCRIPTION

                This function makes the necessary moves the player must make on the board to
                traverse by the appropriate amount of spaces. Before making the move, however,
                it will check to see if adding the position of the player and the spaces to move by
                will throw the position "out of bounds." If so, it will reset the player's
                position to 0 and adjust a_spacesToMove to the appropriate amount of spaces. It will
                also give the player $200, since they pass Go by doing this. Then it adds a_spacesToMove
                and m_playerPosition.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                10:26pm 3/13/2017

        */
        /**/
        public void MakeMove(int a_spacesToMove)
        {
            // Before making the move, check the player's current position on the board.
            // If the adding the spaces to move to the player's position causes it to go
            // over 39, reset the position to 0 and adjust spacesToMove appropriately.
            if ((m_boardPosition + a_spacesToMove) > 39)
            {
                a_spacesToMove = (m_boardPosition + a_spacesToMove) - 40;
                m_boardPosition = 0;
                // Since we're passing (or on) Go, collect $200!
                m_funds += 200;
                // Don't forget to set the boolean of the player passing Go.
                m_passedGo = true;
            }
            // Add the spaces to move to the board position.
            m_boardPosition += a_spacesToMove;
        }
        // Moves the player to jail.
        public void GoToJail()
        {
            // Clear their doubles count.
            m_doublesCount = 0;
            // Change the player's board position to the Just Visiting space.
            m_boardPosition = 10;
            // Make them really in jail by changing the boolean value of m_inJail.
            m_inJail = true;
        }
        /**/
        /*
        MortgageProperty()

        NAME

                MortgageProperty() - mortgages a player's property

        SYNOPSIS

                public void MortgageProperty(string a_property)

                a_property -> the name of the property to mortgage

        DESCRIPTION

                This function mortgages a property that the player owns. First it uses
                FindProperty to get the property in the player's list. Then it gets the
                mortgage value. Afterwards, it mortgages the property by changing the
                corresponding boolean value and adds the mortgage value to the player's
                funds.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                7:35pm 5/14/2017

        */
        /**/
        public void MortgageProperty(string a_property)
        {
            // First, get the property.
            Property mortgageProp = FindProperty(a_property);
            // Now get the property's mortgage value.
            int mortgageValue = mortgageProp.MortgageValue;
            // Mortgage it, then add the mortgage value to the player's funds.
            mortgageProp.IsMortgaged = true;
            m_funds += mortgageValue;
        }
        /**/
        /*
        UnmortgageProperty()

        NAME

                UmnortgageProperty() - unmortgages a player's property... if possible

        SYNOPSIS

                public bool UnmortgageProperty(string a_property)

                a_property -> the name of the property to unmortgage

        DESCRIPTION

                This function attempts to unmortgage a property the player owns. First
                it uses FindProperty to find the property in the player's list. Then
                it gets the unmortgageValue by calculating the mortgage value plus 10%.
                If funds are sufficient, it changes the mortgage boolean to false, subtracts
                funds, and returns true. Otherwise, it returns false.

        RETURNS

                A boolean determining whether or not the property can be unmortgaged.

        AUTHOR

                Bryan Leier

        DATE

                7:42pm 5/14/2017

        */
        /**/
        public bool UnmortgageProperty(string a_property)
        {
             // First, get the property.
            Property unmortgageProp = FindProperty(a_property);
            // Now get the property's mortgage value, plus 10% interest.
            int unmortgageValue = (int)(unmortgageProp.MortgageValue * 1.1);
            // Does the player have sufficient funds to unmortgage?
            if (m_funds >= unmortgageValue)
            {
                // Unmortgage it, then subtract the funds needed to unmortgage the property.
                unmortgageProp.IsMortgaged = false;
                m_funds -= unmortgageValue;
                return true;
            }
            // Otherwise, the property cannot be unmortgaged until it can be paid off.
            return false;
        }

        // Adds a property to the player's list of properties.
        public void AddProperty(ref Property a_property)
        {
            // Make this player the owner and add it to their property list.
            a_property.Owner = this;
            m_properties.Add(a_property);
        }

        // Removes a property from the player's list of properties.
        public void RemoveProperty(ref Property a_property)
        {
            // Change the property's owner and remove it from their property list.
            a_property.Owner = new Player();
            for (int i = 0; i < m_properties.Count; i++)
            {
                // Get the property.
                Property currentProp = (Property)m_properties[i];
                // If this is the property we want to remove, remove it.
                if (currentProp.Name == a_property.Name)
                {
                    m_properties.Remove(currentProp);
                }
            }

        }

        /**/
        /*
        FindProperty()

        NAME

                FindProperty() - finds a property in the player's property list

        SYNOPSIS

                private Property FindProperty(string a_property)

                a_property -> the name of the property to find in the player's property list

        DESCRIPTION

                This function finds a property in the player's property list. It searches through
                the property ArrayList to find it. If the currently visited Property has the same
                name as a_property, it returns the property. Otherwise, it returns a blank property.

        RETURNS

                The property that the function is looking for.

        AUTHOR

                Bryan Leier

        DATE

                7:26pm 5/14/2017

        */
        /**/
        private Property FindProperty(string a_property)
        {
            for (int i = 0; i < m_properties.Count; i++)
            {
                // Get the property.
                Property currentProp = (Property)m_properties[i];
                if (currentProp.Name == a_property) return (Property)m_properties[i];
            }
            // Otherwise, return something else.
            Property blank = new OnlineMonopoly.Property();
            return blank;
        }

        // The player's name.
        private string m_name;
        // The amount of money the player currently has.
        private int m_funds;
        // The token the player is using to represent themselves on the board.
        // (I guess this could be a string for now?)
        private string m_token;
        // The properties that the player owns.
        private ArrayList m_properties;
        // The amount of Get Out of Jail Free cards the player has.
        private int m_getOutOfJailAmount;
        // The amount of turns the player has been in jail for.
        private int m_jailTurnCount;
        // The amount of "doubles" the player has thrown. (2 of the same number)
        private int m_doublesCount;
        // The current position of the player on the board.
        private int m_boardPosition;
        // Determines if the player is in jail or not.
        private bool m_inJail;
        // The amount of houses the player owns.
        private int m_houseAmount;
        // The amount of hotels the player owns.
        private int m_hotelAmount;
        // The amount on the die that the player rolled.
        private int m_dieRoll;
        // The player's "position" in the player roster.
        private int m_rosterPosition;
        // Determines whether or not the player passed Go.
        private bool m_passedGo;
    }
}