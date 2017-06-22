// The Property class, which represents and implements the properties that are in the game.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMonopoly
{
    class Property : Object
    {
        // Default constructor.
        public Property()
        {
            // Just fill in some "default" info for everything...
            m_name = "N/A";
            m_price = 0;
            m_rent = 0;
            m_rent1House = 0;
            m_rent2House = 0;
            m_rent3House = 0;
            m_rent4House = 0;
            m_rentHotel = 0;
            m_mortgageValue = 0;
            m_isMortgaged = false;
            m_buildingCost = 0;
            m_color = "N/A";
            m_owner = new Player();
        }
        // Constructor that takes in the values of all necessary prices and information about a property.
        public Property(string a_name, int a_price, int a_rent, int a_rent1House, int a_rent2House, int a_rent3House,
            int a_rent4House, int a_rentHotel, int a_mortgageValue, int a_buildingCost, string a_color)
        {
            m_name = a_name;
            m_price = a_price;
            m_rent = a_rent;
            m_rent1House = a_rent1House;
            m_rent2House = a_rent2House;
            m_rent3House = a_rent3House;
            m_rent4House = a_rent4House;
            m_rentHotel = a_rentHotel;
            m_mortgageValue = a_mortgageValue;
            m_isMortgaged = false;
            m_buildingCost = a_buildingCost;
            m_color = a_color;
            // Initialized properties will not have an owner, so set it to a "default."
            m_owner = new Player();
        }
        // Properties for the property name.
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        // Properties for the property rents.
        public int Rent
        {
            get
            {
                return m_rent;
            }
            set
            {
                m_rent = value;
            }
        }
        public int Rent1House
        {
            get
            {
                return m_rent1House;
            }
            set
            {
                m_rent1House = value;
            }
        }
        public int Rent2House
        {
            get
            {
                return m_rent2House;
            }
            set
            {
                m_rent2House = value;
            }
        }
        public int Rent3House
        {
            get
            {
                return m_rent3House;
            }
            set
            {
                m_rent3House = value;
            }
        }
        public int Rent4House
        {
            get
            {
                return m_rent4House;
            }
            set
            {
                m_rent4House = value;
            }
        }
        public int RentHotel
        {
            get
            {
                return m_rentHotel;
            }
            set
            {
                m_rentHotel = value;
            }
        }
        // Properties to get the property's price.
        public int Price
        {
            get
            {
                return m_price;
            }
            set
            {
                m_price = value;
            }
        }
        // Properties to get the property's owner.
        public Player Owner
        {
            get
            {
                return m_owner;
            }
            set
            {
                m_owner = value;
            }
        }
        // Properties to get the property's color group.
        public string Color
        {
            get
            {
                return m_color;
            }
        }
        // Properties to get and set the status of the property being mortgaged.
        public bool IsMortgaged
        {
            get
            {
                return m_isMortgaged;
            }
            set
            {
                m_isMortgaged = value;
            }
        }
        // Property to get the property's mortgage value.
        public int MortgageValue
        {
            get
            {
                return m_mortgageValue;
            }
        }
        // Property to get the cost of buildings on the space.
        public int BuildingCost
        {
            get
            {
                return m_buildingCost;
            }
        }

        // The name of the property.
        private string m_name;
        // The price of the property.
        private int m_price;
        // The rent cost of the property.
        private int m_rent;
        // The rent cost of the property with varying house amounts.
        private int m_rent1House;
        private int m_rent2House;
        private int m_rent3House;
        private int m_rent4House;
        // The rent cost of the property with a hotel on it.
        private int m_rentHotel;
        // The mortgage value of the property (what the property can be mortgaged for).
        private int m_mortgageValue;
        // The boolean value that determines if the property is currently mortgaged.
        private bool m_isMortgaged;
        // The price of buildings to put on the property.
        private int m_buildingCost;
        // The "color" of the property. For the sake of simplicity and efficiency, railroads and utilities are considered
        // "colors."
        private string m_color;
        // The "owner" of the property.
        private Player m_owner;
    }
}
