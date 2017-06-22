// The Space class, which represents and implements the spaces that are in the game.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Space : Object
    {
        // Default constructor.
        public Space()
        {
            // Use "default" values.
            m_name = "N/A";
            m_property = new Property();
            m_hasProperty = false;
            m_buildingAmount = 0;
        }
        // Constructor that initializes a property space.
        public Space(Property a_property)
        {
            m_name = a_property.Name;
            m_property = a_property;
            m_hasProperty = true;
            m_buildingAmount = 0;
        }
        // Constructor that initializes a space that does not have a property on it.
        public Space(string a_name)
        {
            m_name = a_name;
            m_property = new Property();
            m_hasProperty = false;
            m_buildingAmount = 0;
        }
        // Properties for the amount of buildings on the space.
        public int BuildingAmount
        {
            get
            {
                return m_buildingAmount;
            }
            set
            {
                m_buildingAmount = value;
            }
        }
        // Properties for whether or not the space is a property space.
        public bool HasProperty
        {
            get
            {
                return m_hasProperty;
            }
            set
            {
                m_hasProperty = value;
            }
        }
        // Gets the property on the space.
        public Property GetProperty()
        {
            return m_property;
        }
        // Property for the space's name.
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /**/
        /*
        CalculateRent()

        NAME

                CalculateRent() - calculates the rent of the property on the space

        SYNOPSIS

                public int CalculateRent()

        DESCRIPTION

                This function calculates the rent of the property on the space by using
                properties defined in the Property class. This is done by looking at the
                value of m_buildingAmount and returning the appropriate rent for the amount
                of buildings on the space.

        RETURNS

                Returns the current rent of the property that is owed by whoever lands on it.

        AUTHOR

                Bryan Leier

        DATE

                4:45pm 2/8/2017

        */
        /**/
        public int CalculateRent()
        {
            // It seems that the best way to do this would be using a switch statement on the amount of buildings
            // that are currently on the space.
            switch(m_buildingAmount)
            {
                case 1: // 1 house
                    return m_property.Rent1House;
                case 2: // 2 houses
                    return m_property.Rent2House;
                case 3: // 3 houses
                    return m_property.Rent3House;
                case 4: // 4 houses
                    return m_property.Rent4House;
                case 5: // hotel
                    return m_property.RentHotel;
                default: // no houses
                    return m_property.Rent;
            }
            // Now, I will need to adjust this function later to account for railroads and utilities, as they
            // do not follow conventional rent calculations.
        }

        /**/
        /*
        AddBuilding()

        NAME

                AddBuilding() - adds a building to the space

        SYNOPSIS

                public void AddBuilding()

        DESCRIPTION

                This function "builds" a house or hotel on the space by incrementing m_buildingAmount.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:04pm 2/8/2017

        */
        /**/
        public void AddBuilding()
        {
            m_buildingAmount++;
        }

        /**/
        /*
        AddBuilding()

        NAME

                RemoveBuilding() - removes a building from the space

        SYNOPSIS

                public void RemoveBuilding()

        DESCRIPTION

                This function removes a house or hotel on the space by decrementing m_buildingAmount.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                6:22pm 2/27/2017

        */
        /**/
        public void RemoveBuilding()
        {
            m_buildingAmount--;
        }

        // The name of the space.
        private string m_name;
        // The property on the space.
        private Property m_property;
        // A boolean that determines whether or not the space has a property.
        private bool m_hasProperty;
        // The amount of buildings on the space.
        private int m_buildingAmount;
    }
}
