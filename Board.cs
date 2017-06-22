// The Board class, which represents the board in the game.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Board : Object
    {
        // The constructor that initializes the board in the game.
        public Board()
        {
            // First, initialize all of the properties:
            Property mediterranean = new Property("Mediterranean Avenue", 60, 2, 10, 30, 90, 160, 250, 30, 50, "Purple");
            Property baltic = new Property("Baltic Avenue", 60, 4, 20, 160, 180, 320, 450, 30, 50, "Purple");
            Property reading = new Property("Reading Railroad", 200, 25, 50, 100, 200, 0, 0, 100, 0, "Railroad");
            Property oriental = new Property("Oriental Avenue", 100, 6, 30, 90, 270, 400, 550, 50, 50, "Light Blue");
            Property vermont = new Property("Vermont Avenue", 100, 6, 30, 90, 270, 400, 550, 50, 50, "Light Blue");
            Property connecticut = new Property("Connecticut Avenue", 120, 8, 40, 100, 300, 450, 600, 60, 50, "Light Blue");
            Property stCharles = new Property("St. Charles Place", 140, 10, 50, 150, 450, 625, 750, 70, 100, "Pink");
            Property electric = new Property("Electric Company", 150, 0, 0, 0, 0, 0, 0, 75, 0, "Utility");
            Property states = new Property("States Avenue", 140, 10, 50, 150, 450, 625, 750, 70, 100, "Pink");
            Property virginia = new Property("Virginia Avenue", 160, 12, 60, 180, 500, 700, 900, 80, 100, "Pink");
            Property pennsylvaniaRailroad = new Property("Pennsylvania Railroad", 200, 25, 50, 100, 200, 0, 0, 100, 0, "Railroad");
            Property stJames = new Property("St. James Place", 180, 14, 70, 200, 550, 750, 950, 90, 100, "Orange");
            Property tennessee = new Property("Tennessee Avenue", 180, 14, 70, 200, 550, 750, 950, 90, 100, "Orange");
            Property newYork = new Property("New York Avenue", 200, 16, 80, 220, 600, 800, 1000, 100, 100, "Orange");
            Property kentucky = new Property("Kentucky Avenue", 220, 18, 90, 250, 700, 875, 1050, 110, 150, "Red");
            Property indiana = new Property("Indiana Avenue", 220, 18, 90, 250, 700, 875, 1050, 110, 150, "Red");
            Property illinois = new Property("Illinois Avenue", 240, 20, 100, 300, 750, 925, 110, 120, 150, "Red");
            Property BO = new Property("B. & O. Railroad", 200, 25, 50, 100, 200, 0, 0, 100, 0, "Railroad");
            Property atlantic = new Property("Atlantic Avenue", 260, 22, 110, 330, 800, 975, 1150, 130, 150, "Yellow");
            Property ventnor = new Property("Ventnor Avenue", 260, 22, 110, 330, 800, 975, 1150, 130, 150, "Yellow");
            Property water = new Property("Water Works", 150, 0, 0, 0, 0, 0, 0, 75, 0, "Utility");
            Property marvin = new Property("Marvin Gardens", 280, 24, 120, 360, 850, 1025, 1200, 140, 150, "Yellow");
            Property pacific = new Property("Pacific Avenue", 300, 26, 130, 390, 900, 1100, 1275, 150, 200, "Green");
            Property northCarolina = new Property("North Carolina Avenue", 300, 26, 130, 390, 900, 110, 1275, 150, 200, "Green");
            Property pennsylvaniaAve = new Property("Pennsylvania Avenue", 320, 28, 150, 450, 1000, 1200, 1400, 160, 200, "Green");
            Property shortLine = new Property("Short Line", 200, 25, 50, 100, 200, 0, 0, 100, 0, "Railroad");
            Property park = new Property("Park Place", 350, 35, 175, 500, 1100, 1300, 1500, 175, 200, "Blue");
            Property boardwalk = new Property("Boardwalk", 400, 50, 200, 600, 1400, 1700, 2000, 200, 200, "Blue");

            // Put those properties into the spaceContainer, along with spaces that are not properties.
            m_spaceContainer = new Space[40] { new Space("GO"), new Space(mediterranean), new Space("Community Chest"),
            new Space(baltic),  new Space("Income Tax"), new Space(reading), new Space(oriental), new Space("Chance"),
            new Space(vermont), new Space(connecticut), new Space("Just Visiting"), new Space(stCharles), new Space(electric),
            new Space(states), new Space(virginia), new Space(pennsylvaniaRailroad), new Space(stJames),
            new Space("Community Chest"), new Space(tennessee), new Space(newYork), new Space("Free Parking"),
            new Space(kentucky), new Space("Chance"), new Space(indiana), new Space(illinois), new Space(BO), new Space(atlantic),
            new Space(ventnor), new Space(water), new Space(marvin), new Space("Go To Jail"), new Space(pacific),
            new Space(northCarolina), new Space("Community Chest"), new Space(pennsylvaniaAve), new Space(shortLine), new Space("Chance"),
            new Space(park), new Space("Luxury Tax"), new Space(boardwalk) };

            // Now add the two decks.
            m_chanceDeck = new Deck("chance");
            m_communityChestDeck = new Deck("community");
            // Shuffle these decks so that we don't always have the same order of cards when starting the game.
            m_chanceDeck.ShuffleDeck();
            m_communityChestDeck.ShuffleDeck();
        }
        /**/
        /*
        SpaceAt()

        NAME

                SpaceAt() - returns a space on the board at an indicated index

        SYNOPSIS

                public int SpaceAt(int a_index)

                a_index -> the index in the m_spaceContainer array to access

        DESCRIPTION

                This function looks for a space on the board that pertains to the appropriate index
                in the array of Spaces stored inside the class. If the range is valid (0-39), it will
                return the space indicated. Otherwise, it will return an empty space and display an
                error message in the console window.

        RETURNS

                Returns a space in m_spaceContainer or an empty space, depending on whether or not the
                validity check passed or not.

        AUTHOR

                Bryan Leier

        DATE

                11:00am 2/27/2017

        */
        /**/
        public Space SpaceAt(int a_index)
        {
            // Only return a space within the appropriate index range (0-39). If it's higher, return an error
            // and an empty space.
            if (a_index >= 0 && a_index <= 39) return m_spaceContainer[a_index];
            else
            {
                Console.WriteLine("Error accessing the space: index" + a_index.ToString() + " is not within the range.");
                return new Space();
            }
        }
        /**/
        /*
        SpaceAt()

        NAME

                BuildOn() - builds a building on the space at an indicated index

        SYNOPSIS

                public int BuildOn(int a_index)

                a_index -> the index in the m_spaceContainer array to access

        DESCRIPTION

                This function adds a building to the space on the board at an indicated index. However,
                the code is smart enough to know that a building should contain no more than a hotel (5 buildings).
                If it detects that the building amount is greater than 5, it displays a message to the console
                window indicating that building on the space is not possible.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                6:32pm 2/27/2017

        */
        /**/
        public void BuildOn(int a_index)
        {
            // Only build something on the space if the amount of buildings are less than 5.
            if (m_spaceContainer[a_index].BuildingAmount < 5) m_spaceContainer[a_index].AddBuilding();
            // Otherwise, return an error in the console window indicating such.
            else Console.WriteLine("You cannot build more than a hotel on a space.");
        }
        // Draws from a specified deck.
        public Card DrawFromDeck(string a_deckName)
        {
            // Draw from the Chance deck if the deck name argument is "Chance." Otherwise, do Community Chest.
            if (a_deckName == "Chance") return m_chanceDeck.GetCardOnTop();
            return m_communityChestDeck.GetCardOnTop();
        }
        // Puts the card at the top of the specified deck on the bottom.
        public void PutCardOnBottom(string a_deckName)
        {
            if (a_deckName == "Chance") m_chanceDeck.MoveTopCardToBottom();
            else m_communityChestDeck.MoveTopCardToBottom();
        }
        
        /**/
        /*
        SpaceAt()

        NAME

                GetSpaceWithProperty() - gets the space with the specified property name

        SYNOPSIS

                public Space GetSpaceWithProperty(string a_propertyName)

                a_propertyName -> the name of the property to get the space of

        DESCRIPTION

                This function gets a space on the board that has the property in the argument.
                It does this by searching in the space container within the board. If the space's
                property has the same name as a_propertyName, it will return the Space. This should
                always return a space due to the nature of how this is programmed, but just in case
                it doesn't, it returns an empty space. Just to avoid bellyaching!

        RETURNS

                Returns the Space that contains the property the function is looking for.

        AUTHOR

                Bryan Leier

        DATE

                8:31pm 5/25/2017

        */
        /**/
        public Space GetSpaceWithProperty(string a_propertyName)
        {
            // Search the board for the space we're looking for.
            for (int i = 0; i < 40; i++)
            {
                // If this is the space, return it.
                if (m_spaceContainer[i].Name == a_propertyName)
                {
                    return m_spaceContainer[i];
                }
            }
            // If it was not found, return an empty space.
            return new Space();
        }

        // An array of spaces that represent the game board.
        private Space[] m_spaceContainer;
        // Decks for Chance and Community Chest.
        private Deck m_chanceDeck;
        private Deck m_communityChestDeck;
    }
}