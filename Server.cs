// The Server class, which is what hosts the game and interacts with clients.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Collections;

namespace OnlineMonopoly
{
    class Server
    {
    	// Constructor for the Server.
    	public Server()
    	{
    		// Set up the socket.
    		m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    		// Set up the list.
    		m_clientSockets = new List<Socket>();
            // Set up the game.
            m_game = new Game();
            // The game hasn't started yet, so...
            m_gameStarted = false;
    	}
    	/**/
        /*
        SetUpServer()

        NAME

                SetUpServer() - buys a property

        SYNOPSIS

                public void SetUpServer()

        DESCRIPTION

                This function sets up the server to clients can connect. It uses the server's
                socket to bind, start listening for new connections, and begin accepting new
                connections in an asynchronous manner.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:40pm 4/8/2017

        */
        /**/
    	public void SetUpServer()
    	{
    		// Tell the console window we are setting up the server.
    		Console.WriteLine("Setting up the game server...");
    		// Set up the server.
    		m_serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
    		m_serverSocket.Listen(1);
    		m_serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
    	}
        // Command enumeration. These Commands are used to communicate what actions should be performed between
        // the clients and the server. The Commands serve the following functions:
        // Yes: Indicates a success
        // No: Indicates a failure
        // Join: Join the game
        // Roll: Roll the dice
        // Chat: Write a message to the chat
        // Done: Finish a turn
        // Start: Starts the game
        // NoStart: Indicates a game cannot be started yet
        // TurnStart: Indicates the start of a turn
        // PositionUpdate: Updates player's positions on the board
        // FundUpdate: Updates the amount of funds a player has
        // ShowMessageBox: Shows a message to the client in a MessageBox
        // RentBox: Shows a message about rent to a client
        // BuyProperty: Buys a property
        // Mortgage: Mortgages a property
        // Unmortgage: Unmortgages a property
        // Tax: Indicates tax must be paid
        // Jail: Sends someone to jail
        // Card: Sends information about a card
        // GetNames: Gets the names of players in the game
        // GetProperties: Gets a list of properties eligible to be traded
        // GetFunds: Gets funds for players
        // TradeRequest: Sends a trade request
        // AcceptTrade: Accepts a trade
        // DeclineTrade: Declines a trade
        // GetBuildingProperties: Gets properties to build on
        // GetBuildngInfo: Gets information about building on a property (cost, amount of buildings)
        // BuyBuilding: Buys a building
        // SellBuilding: Sells a building
        // BankruptWarning: Warns a player that they are bankrupt
        // Bankrupt: Bankrupts a player
        // Winner: Indicates who won the game
        // None: Default command
        public enum Command
        {
            Yes, No, Join, Roll, Chat, Done, Start, NoStart, TurnStart, PositionUpdate, FundUpdate, ShowMessageBox,
            RentBox, BuyProperty, Mortgage, Unmortgage, Tax, Jail, Card, GetNames, GetProperties, GetFunds,
            TradeRequest, AcceptTrade, DeclineTrade, GetBuildingProperties, GetBuildingInfo, BuyBuilding,
            SellBuilding, BankruptWarning, Bankrupt, Winner, None
        }
        // The PlayerCommand structure that is sent between the client and servers as an easy method of communication.
        // It contains a Name, a Command, and a Message. It uses Marshaling to properly allocate data for ease of
        // byte conversion later.
        public struct PlayerCommand
        {
            public string Name
            {
                get
                {
                    return s_name;
                }
                set
                {
                    s_name = value;
                }
            }
            public Command Command
            {
                get
                {
                    return s_command;
                }
                set
                {
                    s_command = value;
                }
            }
            public string Message
            {
                get
                {
                    return s_message;
                }
                set
                {
                    s_message = value;
                }
            }
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25)]
            private string s_name;
            private Command s_command;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 510)]
            private string s_message;
        }
        /**/
        /*
        GetBytes()

        NAME

                GetBytes() - converts a PlayerCommand struct into a byte array

        SYNOPSIS

                private byte[] GetBytes(PlayerCommand a_command)

        DESCRIPTION

                This function uses marshaling to convert a PlayerCommand structure to a byte
                array.

        RETURNS

                A byte array containing the structure that it converted.

        AUTHOR

                Vincent McNabb

        DATE

                6:21pm 4/13/2017

        */
        /**/
        private byte[] GetBytes(PlayerCommand a_command)
        {
            // Get the size of the struct.
            int size = Marshal.SizeOf(a_command);
            byte[] data = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(a_command, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
            return data;
        }
        /**/
        /*
        GetStruct()

        NAME

                GetStruct() - converts a byte array into a struct

        SYNOPSIS

                private PlayerCommand GetStruct(byte[] a_bytes)

        DESCRIPTION

                This function uses marshaling to convert a byte array into a PlayerCommand
                struct.

        RETURNS

                A PlayerCommand structure containing data from the byte array it converted.

        AUTHOR

                Vincent McNabb

        DATE

                6:24pm 4/13/2017

        */
        /**/
        private PlayerCommand GetStruct(byte[] a_bytes)
        {
            PlayerCommand structure = new PlayerCommand();

            int size = Marshal.SizeOf(structure);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(a_bytes, 0, ptr, size);
            structure = (PlayerCommand)Marshal.PtrToStructure(ptr, structure.GetType());
            Marshal.FreeHGlobal(ptr);

            return structure;
        }
        /**/
        /*
        AcceptCallback()

        NAME

                AcceptCallback() - accepts new connections to the server

        SYNOPSIS

                private void AcceptCallback(IAsyncResult AR)

                AR -> the status of an asynchronous operation

        DESCRIPTION

                This function accepts clients and lets them connect to the server. First, it accepts the client.
                Then it begins receiving information from the client pertaining to the game. It adds the socket
                to the list of clients afterwards. Due to the nature of Monopoly (8 player maximum), the server
                stops accepting new connections once 8 clients are connected.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:40pm 4/8/2017

        */
        /**/
    	private void AcceptCallback(IAsyncResult AR)
    	{
    		// Accept a client.
    		Socket socket = m_serverSocket.EndAccept(AR);
            // Receive the join command.
            socket.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            // Add the socket to the list of clients.
            m_clientSockets.Add(socket);
            // Say a client has connected.
            Console.WriteLine("Client connected!");
            // How many clients are connected? If it's less than 8, accept more.
            if (m_clientSockets.Count < 8) m_serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            // Otherwise, no more players can enter the game. Don't accept more.
    	}
        /**/
        /*
        ReceiveCallback()

        NAME

                ReceiveCallback() - receives information sent from the client

        SYNOPSIS

                private void ReceiveCallback(IAsyncResult AR)

                AR -> the status of an asynchronous operation

        DESCRIPTION

                This function receives an array of bytes send by the client. It transforms the
                bytes received into the PlayerCommand structure using GetStruct and proceeds to
                perform actions based on the Command that was sent through the structure. This is
                achieved through an admittedly large switch statement. After performing the appropriate
                actions, the server will start accepting information from the client once more. If
                it detects a player has disconnected, it will perform appropriate disconnect actions
                depending on the amount of players left in the game.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:51pm 4/8/2017

        */
        /**/
    	private void ReceiveCallback(IAsyncResult AR)
    	{
    		// Get the socket that is sending information to the server.
    		Socket socket = (Socket)AR.AsyncState;
    		try
    		{
				int bytesReceived = socket.EndReceive(AR);
	    		byte[] dataBuffer = new byte[bytesReceived];
	    		// Transfer what is in the server's buffer to this new dataBuffer.
	    		Array.Copy(m_buffer, dataBuffer, bytesReceived);
	    		// Convert the dataBuffer to a PlayerCommand struct.
	    		PlayerCommand command = GetStruct(dataBuffer);
	    		// Write a message to the console displaying what was sent.
	    		Console.WriteLine("Command received from " + command.Name);
	    		// Another switch statement! Yay!
	    		switch(command.Command)
	    		{
	    			case Command.Chat:
	    				// Send this message to all of the other players.
	    				SendToAll(dataBuffer);
	    				break;
                    case Command.Join:
                        // Process the join.
                        ProcessJoin(command.Name, ref socket);
                        break;
                    case Command.Roll:
                        // The player is taking their turn, so roll the dice.
                        Player turnTaker = m_game.GetPlayer(command.Name);
                        RollDice(ref turnTaker);
                        break;
                    case Command.Start:
                        // Process the start.
                        ProcessStart(dataBuffer, ref socket);
                        break;
                    case Command.Done:
                        // Process the next turn.
                        NextTurn();
                        break;
                    case Command.FundUpdate:
                        // Send the player their funds.
                        SendPlayerFunds(command.Name);
                        break;
                    case Command.BuyProperty:
                        // Perform the operations to buy a property.
                        Player buyer = m_game.GetPlayer(command.Name);
                        BuyPropertyActions(ref buyer);
                        // Send fund updates to the player.
                        SendPlayerFunds(command.Name);
                        break;
                    case Command.Mortgage:
                        // Perform the operations of mortgaging a property.
                        MortgageProperty(command.Name, command.Message);
                        // Send fund updates to the player.
                        SendPlayerFunds(command.Name);
                        break;
                    case Command.Unmortgage:
                        // Perform unmortgaging operations.
                        if (UnmortgageProperty(command.Name, command.Message))
                        {
                            // Indicate that the unmortgaging was a success to the player.
                            command.Name = "yes";
                        }
                        else
                        {
                            // Indicate that the unmortgaging was not a success.
                            command.Name = "no";
                        }
                        byte[] unmortgageResponse = GetBytes(command);
                        socket.BeginSend(unmortgageResponse, 0, unmortgageResponse.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                        break;
                    case Command.Jail:
                        // The player that went to jail now knows, so send position updates.
                        SendPositionUpdates();
                        break;
                    case Command.Card:
                        // The action on the card must now be performed.
                        DoCardActions(command.Name, command.Message);
                        break;
                    case Command.GetNames:
                        // Get the names of players still in the game.
                        SendPlayerNames(command.Name);
                        break;
                    case Command.GetProperties:
                        // Start getting the properties of the requested player.
                        SendProperties(command.Name, command.Message);
                        break;
                    case Command.GetFunds:
                        // Send the funds of a requested player and the player themselves.
                        SendTradeFunds(command.Name, command.Message);
                        break;
                    case Command.TradeRequest:
                        // Start processing a trade request.
                        DoTradeRequest(command.Name, command.Message);
                        break;
                    case Command.AcceptTrade:
                        // Start processing the trade.
                        ProcessTrade(command.Name, command.Message);
                        break;
                    case Command.DeclineTrade:
                        // Send a DeclineTrade command to the player in the message.
                        DeclinedTrade(command.Message);
                        break;
                    case Command.GetBuildingProperties:
                        // Start sending a list of eligible properties to build on.
                        SendPropertiesToBuildOn(command.Name);
                        break;
                    case Command.GetBuildingInfo:
                        // Start sending info about buildings on the property.
                        SendBuildingInfo(command.Name, command.Message);
                        break;
                    case Command.BuyBuilding:
                        // Buy a building.
                        BuyBuildingActions(command.Name, command.Message);
                        break;
                    case Command.SellBuilding:
                        // Sell a building.
                        SellBuildingActions(command.Name, command.Message);
                        break;
                    case Command.Bankrupt:
                        // Bankrupt the player.
                        BankruptPlayerActions(command.Name);
                        break;
	    			default:
	    				break;
	    		}
	    		// Start receiving again, but only if it's connected!
                if (socket.Connected) socket.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
    		catch(SocketException)
    		{
    			Console.WriteLine("Client disconnected.");
                // This player is out of the game. We will act like they were bankrupted and liquidated.
                // Before doing anything, look at the client sockets list. Is there only one player left?
                if (m_clientSockets.Count == 1)
                {
                    // Close the socket and remove them from the list.
                    socket.Close();
                    m_clientSockets.Remove(socket);
                    // Refresh the game.
                    m_game = new Game();
                    m_gameStarted = false;
                    Console.WriteLine("New game state has been started!");
                    return;
                }
                Player disconnectedPlayer = new Player();
                for (int i = 0; i < m_clientSockets.Count; i++)
                {
                    if (m_clientSockets[i] == socket)
                    {
                        // Get the player at this position.
                        disconnectedPlayer = m_game.GetPlayerAt(i);
                    }
                }
    			// Close the socket and remove them from the list.
    			socket.Close();
    			m_clientSockets.Remove(socket);
                // If the disconnectedPlayer's name is still "N/A," they already went bankrupt. Just return.
                if (disconnectedPlayer.Name == "N/A") return;
                // Send a chat message saying a player disconnected.
                PlayerCommand disconnecter = new PlayerCommand();
                disconnecter.Command = Command.Chat;
                disconnecter.Message = "(" + disconnectedPlayer.Name + " has disconnected from the server.)";
                byte[] disconnectBytes = GetBytes(disconnecter);
                SendToAll(disconnectBytes);
                // Bankrupt the player.
                BankruptPlayerActions(disconnectedPlayer.Name);
    		}
    	}
        /**/
        /*
        SendCallback()

        NAME

                SendCallback() - sends information to the client

        SYNOPSIS

                private void SendCallback(IAsyncResult AR)

                AR -> the status of an asynchronous operation

        DESCRIPTION

                This function sends information to a client. Once the information is sent, EndSend is called.
                Not too much to describe here!

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:56pm 4/8/2017

        */
        /**/
    	private void SendCallback(IAsyncResult AR)
    	{
    		try
    		{
    			Socket socket = (Socket)AR.AsyncState;
    			socket.EndSend(AR);
    		}
    		catch
    		{
    			Console.WriteLine("Error in SendCallback");
    		}
    	}
        // Sends an array of bytes to all of the clients connected to the server.
        private void SendToAll(byte[] a_buffer)
        {
            foreach (Socket connectedClient in m_clientSockets)
                connectedClient.BeginSend(a_buffer, 0, a_buffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), connectedClient);
        }
        /**/
        /*
        ProcessJoin()

        NAME

                ProcessJoin() - processes a player joining the game

        SYNOPSIS

                private void ProcessJoin(string a_name, ref Socket a_socket)

                a_name -> the name of the player trying to join the game
                a_socket -> the socket trying to conect to the server

        DESCRIPTION

                This function processes a Join command received from a player. If the
                game has already started or the name already exists, the player cannot
                join the game and will have a No command sent to them and their socket
                is closed and removed from the client sockets list. Otherwise they can
                join the game! Add them to the game and send a Yes command to tell them
                they successfully joined.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                6:01pm 4/10/2017

        */
        /**/
        private void ProcessJoin(string a_name, ref Socket a_socket)
        {
            // Create a response communication.
            PlayerCommand joinResponse = new PlayerCommand();
            // Has the game already started?
            if (m_gameStarted)
            {
                // If so, they cannot join the game. Boot them out.
                joinResponse.Command = Command.No;
                joinResponse.Message = "The game has already started.";
                byte[] startedBuffer = GetBytes(joinResponse);
                a_socket.BeginSend(startedBuffer, 0, startedBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), a_socket);
                a_socket.Close();
                m_clientSockets.Remove(a_socket);
            }
            // Does this player's name already exist in the game?
            else if (m_game.PlayerExistsInRoster(a_name))
            {
                // If so, they cannot join the game. Boot them out.
                joinResponse.Command = Command.No;
                joinResponse.Message = "The name you are trying to join with has already been taken.";
                byte[] noBuffer = GetBytes(joinResponse);
                a_socket.BeginSend(noBuffer, 0, noBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), a_socket);
                a_socket.Close();
                m_clientSockets.Remove(a_socket);
            }
            // Otherwise, go ahead and add them to the game. Send back a response indicating all went well, too.
            else
            {
                Player newPlayer = new Player(a_name, "default");
                m_game.AddPlayer(newPlayer);
                newPlayer.RosterPosition = m_clientSockets.Count - 1;
                joinResponse.Command = Command.Yes;
                byte[] yesBuffer = GetBytes(joinResponse);
                a_socket.BeginSend(yesBuffer, 0, yesBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), a_socket);
            }
        }
        /**/
        /*
        ProcessStart()

        NAME

                ProcessStart() - processes a player starting the game

        SYNOPSIS

                private void ProcessStart(byte[] a_buffer, ref Socket a_socket)

                a_buffer -> the buffer containing PlayerCommand information
                a_socket -> the socket sending the command

        DESCRIPTION

                This function processes a Start command received from one of the
                game's players. First, it determines if a game can actually be started.
                If there is more than one player currently connected, the game will start.
                It sends a Start command to all of the clients (to turn off the Start Game button)
                and then sends a TurnStart command to the first socket connected to the server, as
                they are the first player. Otherwise, a NoStart command is sent back to the
                client. They cannot start the game if they are the only person in the server.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                6:24pm 4/10/2017

        */
        /**/
        private void ProcessStart(byte[] a_buffer, ref Socket a_socket)
        {
            // Initialize a PlayerCommand.
            PlayerCommand startResponse = new PlayerCommand();
            // Is there more than one player ready to play the game?
            if (m_clientSockets.Count > 1)
            {
                // If so, go ahead and start the game!
                // Stop accepting new connections, as the game is beginning.
                m_gameStarted = true;
                // Send a Start command to all of the clients. This turns off the Start Game button.
                SendToAll(a_buffer);
                // Now send a TurnStart command to the first socket that connected to the server. They are the first player.
                startResponse.Command = Command.TurnStart;
                byte[] startTurn = GetBytes(startResponse);
                m_clientSockets[0].Send(startTurn, 0, startTurn.Length, SocketFlags.None);
            }
            // Otherwise, send a message to the player trying to start the game to wait it out.
            else
            {
                startResponse.Command = Command.NoStart;
                byte[] noStartBuffer = GetBytes(startResponse);
                a_socket.BeginSend(noStartBuffer, 0, noStartBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), a_socket);
            }
        }
        /**/
        /*
        RollDice()

        NAME

                RollDice() - lets a player roll the dice

        SYNOPSIS

                private void RollDice(ref Player a_player)

                a_player -> the player rolling the dice

        DESCRIPTION

                This function rolls the dice for a player specified in the argument. It does this
                by calling the RollDice function in the Player class, then sends this roll to
                all the clients so it can be displayed to them. It also sends a chat message that
                indicates what the player rolled. Afterwards, the player can take their turn in DoTurn.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                12:07pm 4/12/2017

        */
        /**/
        private void RollDice(ref Player a_player)
        {
            // Initialize integers for the die roll and roll for them.
            int firstDie = 0, secondDie = 0;
            a_player.RollDice(ref firstDie, ref secondDie);
            // Send these die rolls to all the clients (everyone should see die rolls), along with the doubles count.
            PlayerCommand dieSender = new PlayerCommand();
            dieSender.Name = a_player.Name;
            dieSender.Command = Command.Roll;
            dieSender.Message = firstDie.ToString() + "," + secondDie.ToString() + "," + a_player.DoublesCount;
            byte[] dieBuffer = GetBytes(dieSender);
            SendToAll(dieBuffer);
            // Process the player's turn.
            DoTurn(ref a_player);
        }
        /**/
        /*
        DoTurn()

        NAME

                DoTurn() - lets a player roll the dice

        SYNOPSIS

                private void DoTurn(ref Player a_player)

                a_player -> the player doing their turn

        DESCRIPTION

                This function performs a turn for the player. This is done by calling DoTurn in the Game class, then performing
                appropriate actions, such as sending position updates and appropriate Go actions. Afterwards, the space they
                landed on will be evaluated. The appropriate function to call upon the string returned from EvaluateSpace
                is done in a switch statement. Afterwards, a bankruptcy check is performed to see if anyone is bankrupt.
                The appropriate action for bankruptcy is taken as well, should it be necessary.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                12:42pm 4/12/2017

        */
        /**/
        private void DoTurn(ref Player a_player)
        {
            // Do their turn.
            m_game.DoTurn(ref a_player);
            // Send position updates.
            SendPositionUpdates();
            // Did a player pass Go?
            if (a_player.PassedGo)
            {
                // Send fund updates.
                SendPlayerFunds(a_player.Name);
                // Correct the value.
                a_player.PassedGo = false;
                // Sleep a bit.
                System.Threading.Thread.Sleep(15);
            }
            // Evaluate the space.
            switch (m_game.EvaluateSpace(ref a_player))
            {
                case "pay up":
                    // Pay up the rent, as it is owed.
                    RentActions(ref a_player);
                    break;
                case "wanna buy":
                    // Ask if the player wants to buy a property.
                    AskToBuyProperty(a_player);
                    break;
                case "income tax":
                    // The player must pay a tax for landing on the space.
                    PayTax(a_player, "income tax");
                    break;
                case "luxury tax":
                    // The player must pay a tax for landing on the space.
                    PayTax(a_player, "luxury tax");
                    break;
                case "jail":
                    // The player was sent to jail.
                    WentToJail(a_player);
                    break;
                case "chance":
                    // Draw a Chance card.
                    Card drawnChance = m_game.DrawCard(ref a_player, "Chance");
                    // Send the info.
                    SendCardInfo(a_player, drawnChance.Action, drawnChance.Description);
                    break;
                case "community chest":
                    // Draw a Community Chest card.
                    Card drawnCommunity = m_game.DrawCard(ref a_player, "Community Chest");
                    // Send the info.
                    SendCardInfo(a_player, drawnCommunity.Action, drawnCommunity.Description);
                    break;
                default:
                    break;
            }
            // Perform bankruptcy checks.
            string bankruptcyCheckResult = m_game.BankruptcyCheck();
            // Someone is bankrupt if the result from the function is not null.
            if (bankruptcyCheckResult != "null")
            {
                // Get this player and send them a BankruptWarning.
                // Sleep to prevent a connection error.
                System.Threading.Thread.Sleep(20);
                Player bankruptPlayer = m_game.GetPlayer(bankruptcyCheckResult);
                PlayerCommand bankruptSender = new PlayerCommand();
                bankruptSender.Command = Command.BankruptWarning;
                byte[] bankruptBytes = GetBytes(bankruptSender);
                m_clientSockets[bankruptPlayer.RosterPosition].BeginSend(bankruptBytes, 0, bankruptBytes.Length, SocketFlags.None,
                    new AsyncCallback(SendCallback), null);
            }
        }
        /**/
        /*
        RentActions()

        NAME

                RentActions() - performs rent actions between players

        SYNOPSIS

                private void RentActions(ref Player a_player)

                a_player -> the player paying rent

        DESCRIPTION

                This function performs rent actions between appropriate players. It calculates the rent owed on the space
                the player is on. It exchanges funds between the players, builds the appropriate rent message box dialogues
                for each player, then sends them to the respective players.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                2:05pm 4/12/2017

        */
        /**/
        private void RentActions(ref Player a_player)
        {
            // Pay up the rent, as it is owed.
            int rentOwed = m_game.CalculateRent(m_game.GetSpacePlayerIsOn(a_player), a_player.DieRoll);
            // Generate different message box dialogs for rent owed and rent received.
            string rentOwedDialog = m_game.GetSpacePlayerIsOn(a_player).GetProperty().Owner.Name + " owns " + m_game.GetSpacePlayerIsOn(a_player).GetProperty().Name +
            ". Rent owed is $" + rentOwed.ToString() + ".";
            string rentReceivedDialog = a_player.Name + " landed on " + m_game.GetSpacePlayerIsOn(a_player).GetProperty().Name + ". You receive $" +
            rentOwed.ToString() + "!";
            // Exchange funds.
            Player owner = m_game.GetSpacePlayerIsOn(a_player).GetProperty().Owner;
            m_game.PayUp(rentOwed, ref a_player, ref owner);
            // Send the messages to the respective players.
            PlayerCommand rentSend = new PlayerCommand();
            rentSend.Name = "Rent Owed";
            rentSend.Command = Command.RentBox;
            rentSend.Message = rentOwedDialog;
            // Send this to the poor sap paying rent.
            byte[] rentOwedBuff = GetBytes(rentSend);
            m_clientSockets[a_player.RosterPosition].BeginSend(rentOwedBuff, 0, rentOwedBuff.Length, SocketFlags.None, new AsyncCallback(SendCallback),
                null);
            rentSend.Name = "Rent Received";
            rentSend.Message = rentReceivedDialog;
            // Send this to the one getting rich.
            byte[] rentReceivedBuff = GetBytes(rentSend);
            m_clientSockets[owner.RosterPosition].BeginSend(rentReceivedBuff, 0, rentReceivedBuff.Length, SocketFlags.None, new AsyncCallback(SendCallback),
                null);
            // Sleep for a little to avoid connection errors.
            System.Threading.Thread.Sleep(50);
        }
        /**/
        /*
        AskToBuyProperty()

        NAME

                AskToBuyProperty() - asks the player if they want to buy a property

        SYNOPSIS

                private void AskToBuyProperty(Player a_player)

                a_player -> the player to ask

        DESCRIPTION

                This function asks a player if they want to buy a property. It gets the property they landed on
                then starts generating a message to send to the player to determine if they want to go through
                with buying the property or not. It sends the command afterwards.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                1:18pm 4/14/2017

        */
        /**/
        private void AskToBuyProperty(Player a_player)
        {
            // Get the property the player landed on and start generating the dialog box.
            Property propertyLandedOn = m_game.GetSpacePlayerIsOn(a_player).GetProperty();
            // Generate a PlayerCommand to communicate with the client.
            PlayerCommand wannaBuyCommand = new PlayerCommand();
            wannaBuyCommand.Name = propertyLandedOn.Name;
            wannaBuyCommand.Command = Command.BuyProperty;
            wannaBuyCommand.Message = "Nobody currently owns " + propertyLandedOn.Name + ".\nIt costs $" + propertyLandedOn.Price.ToString() + ". Would you like to buy it?";
            // Send this to the player on the space.
            byte[] wannaBuyBytes = GetBytes(wannaBuyCommand);
            m_clientSockets[a_player.RosterPosition].BeginSend(wannaBuyBytes, 0, wannaBuyBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback),
                null);
        }
        // Lets the player buy the property that the player is currently on.
        private void BuyPropertyActions(ref Player a_player)
        {
            // Get the property the player is on.
            Property spaceProperty = m_game.GetSpacePlayerIsOn(a_player).GetProperty();
            a_player.BuyProperty(ref spaceProperty);
        }
        /**/
        /*
        MortgageProperty()

        NAME

                MortgageProperty() - mortgages a player's property

        SYNOPSIS

                private void MortgageProperty(string a_player, string a_property)

                a_player -> the player mortgaging a property
                a_property -> the name of the property to mortgage

        DESCRIPTION

                This function mortgages a property from the player's properties. It only mortgages
                the property if it's able to, however (cannot be mortgaged if there is at least one
                building on it). If it can't, it sends a command indicating as such to the player.
                Otherwise, it mortgages the property.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                1:37pm 4/14/2017

        */
        /**/
        private void MortgageProperty(string a_player, string a_property)
        {
            // Get the player, then call the MortgageProperty function on them.
            Player player = m_game.GetPlayer(a_player);
            // First, check to see if the property is able to be mortgaged. If the space the property has contains
            // at least one building, it cannot be mortgaged.
            if (m_game.GetSpaceWithProperty(a_property).BuildingAmount > 0)
            {
                // This cannot be mortgaged.
                PlayerCommand noMortgage = new PlayerCommand();
                noMortgage.Command = Command.Mortgage;
                noMortgage.Message = a_property;
                // Send the message.
                byte[] noMortgageBytes = GetBytes(noMortgage);
                m_clientSockets[player.RosterPosition].BeginSend(noMortgageBytes, 0, noMortgageBytes.Length, SocketFlags.None,
                    new AsyncCallback(SendCallback), null);
                return;
            }
            player.MortgageProperty(a_property);
        }
        // Unmortgages a property... ideally!
        private bool UnmortgageProperty(string a_player, string a_property)
        {
            // Get the player.
            Player player = m_game.GetPlayer(a_player);
            return player.UnmortgageProperty(a_property);
        }
        /**/
        /*
        PayTax()

        NAME

                PayTax() - informs a player that a tax has to be paid

        SYNOPSIS

                private void PayTax(Player a_player, string a_rentType)

                a_player -> the player paying a tax
                a_rentType -> the type of rent the player has to pay

        DESCRIPTION

                This function sends a command to the player that indicates
                they must pay a tax. It generates a PlayerCommand with the
                type of tax to pay in the name, then sends the command over
                to the player.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                3:28pm 4/14/2017

        */
        /**/
        private void PayTax(Player a_player, string a_rentType)
        {
            // Generate a PlayerCommand to communicate with the client.
            PlayerCommand taxPay = new PlayerCommand();
            taxPay.Command = Command.Tax;
            // Change the name depending on Income Tax or Luxury Tax.
            if (a_rentType == "income tax") taxPay.Name = "income tax";
            else taxPay.Name = "luxury tax";
            // Send the command to the player.
            byte[] taxBytes = GetBytes(taxPay);
            m_clientSockets[a_player.RosterPosition].BeginSend(taxBytes, 0, taxBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        // Informs the player that they were sent to jail.
        private void WentToJail(Player a_player)
        {
            // Generate a PlayerCommand.
            PlayerCommand sentToJail = new PlayerCommand();
            sentToJail.Command = Command.Jail;
            // Send the command over.
            byte[] jailBytes = GetBytes(sentToJail);
            m_clientSockets[a_player.RosterPosition].BeginSend(jailBytes, 0, jailBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendCardInfo()

        NAME

                SendCardInfo() - sends info about a card that was drawn to a player

        SYNOPSIS

                private void SendCardInfo(Player a_player, string a_action, string a_description)

                a_player -> the player to have the card info sent to
                a_action -> the card action
                a_description -> the card description

        DESCRIPTION

                This function sends information about a card the player drew to the player.
                It compiles everything about the card within the PlayerCommand structure
                then sends it over.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                5:20pm 4/22/2017

        */
        /**/
        private void SendCardInfo(Player a_player, string a_action, string a_description)
        {
            // Generate a PlayerCommand.
            PlayerCommand cardInfo = new PlayerCommand();
            cardInfo.Name = a_action;
            cardInfo.Command = Command.Card;
            cardInfo.Message = a_description;
            // Send the command over.
            byte[] cardBytes = GetBytes(cardInfo);
            m_clientSockets[a_player.RosterPosition].BeginSend(cardBytes, 0, cardBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
            // Sleep for a little bit?
            System.Threading.Thread.Sleep(50);
        }
        /**/
        /*
        SendCardInfo()

        NAME

                SendCardInfo() - sends info about a card that was drawn to a player

        SYNOPSIS

                private void DoCardActions(string a_player, string a_action)

                a_player -> the player that needs the card action to be done to
                a_action -> the card action to perform

        DESCRIPTION

                This function peforms the action associated with the card they
                drew. It uses a switch statement to determine which function
                to perform and what action to take. It also performs a bankruptcy
                check as it's possible to be bankrupted from a card if you are
                unfortunate enough.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                4:15pm 4/28/2017

        */
        /**/
        private void DoCardActions(string a_player, string a_action)
        {
            // Get the player from the name.
            Player player = m_game.GetPlayer(a_player);
            // Perform the card action on them.
            m_game.PerformCardAction(ref player, a_action);
            // Some spaces need different follow ups.
            switch (a_action)
            {
                case "advanceIllinois":
                case "advanceStCharles":
                case "advanceUtility":
                case "advanceRailroad":
                case "goBack3":
                case "readingRailroad":
                case "boardwalk":
                // Send position updates.
                SendPositionUpdates();
                // Evaluate the space.
                switch (m_game.EvaluateSpace(ref player))
                {
                    case "pay up":
                        // Pay up the rent, as it is owed.
                        RentActions(ref player);
                        break;
                    case "wanna buy":
                        // Ask if the player wants to buy a property.
                        AskToBuyProperty(player);
                        break;
                    case "income tax":
                        // The player must pay a tax for landing on the space.
                        PayTax(player, "income tax");
                        break;
                    case "luxury tax":
                        // The player must pay a tax for landing on the space.
                        PayTax(player, "luxury tax");
                        break;
                    case "chance":
                        // Draw a Chance card.
                        Card drawnChance = m_game.DrawCard(ref player, "Chance");
                        // Send the info.
                        SendCardInfo(player, drawnChance.Action, drawnChance.Description);
                        break;
                    case "community chest":
                        // Draw a Community Chest card.
                        Card drawnCommunity = m_game.DrawCard(ref player, "Community Chest");
                        // Send the info.
                        SendCardInfo(player, drawnCommunity.Action, drawnCommunity.Description);
                        break;
                    default:
                        break;
                }
                break;

                case "advanceGo":
                    // Send position updates and fund updates to the player.
                    SendPositionUpdates();
                    SendPlayerFunds(a_player);
                    break;

                case "jail":
                    // Send position updates.
                    SendPositionUpdates();
                    break;

                case "bankDividend":
                case "generalRepairs":
                case "poorTax":
                case "matures":
                case "bankError":
                case "doctorsFee":
                case "stockSale":
                case "xmasFund":
                case "taxRefund":
                case "lifeInsurance":
                case "hospitalFees":
                case "schoolTax":
                case "services":
                case "streetRepairs":
                case "beautyContest":
                case "inherit100":
                    // Send fund updates to the player.
                    SendPlayerFunds(a_player);
                    break;

                case "chairman":
                case "grandOpera":
                    // Send fund updates to everyone.
                    List<string> playerList = m_game.GetPlayers();
                    foreach (string name in playerList)
                    {
                        SendPlayerFunds(name);
                    }
                    break;

                default:
                    break;
            }
            // Perform bankruptcy checks.
            string bankruptcyCheckResult = m_game.BankruptcyCheck();
            // Someone is bankrupt if the result from the function is not null.
            if (bankruptcyCheckResult != "null")
            {
                // Get this player and send them a BankruptWarning.
                Player bankruptPlayer = m_game.GetPlayer(bankruptcyCheckResult);
                PlayerCommand bankruptSender = new PlayerCommand();
                bankruptSender.Command = Command.BankruptWarning;
                byte[] bankruptBytes = GetBytes(bankruptSender);
                m_clientSockets[bankruptPlayer.RosterPosition].BeginSend(bankruptBytes, 0, bankruptBytes.Length, SocketFlags.None,
                    new AsyncCallback(SendCallback), null);
            }
        }
        // Processes a Done command received from a player and shifts control to the next player to take their turn.
        private void NextTurn()
        {
            // Increment the player position.
            m_game.PlayerPosition = m_game.PlayerPosition + 1;
            // Reset this position if it's the same number as the amount of players still playing (not bankrupt).
            if (m_game.PlayersStillPlaying == m_game.PlayerPosition) m_game.PlayerPosition = 0;
            // Send a TurnStart command to this player.
            PlayerCommand turn = new PlayerCommand();
            turn.Command = Command.TurnStart;
            byte[] startTurn = GetBytes(turn);
            m_clientSockets[m_game.GetPlayerAt(m_game.PlayerPosition).RosterPosition].Send(startTurn, 0, startTurn.Length, SocketFlags.None);
        }
        // Sends the board position of all players.
        private void SendPositionUpdates()
        {
            // Send the position updates.
            PlayerCommand posUpdate = new PlayerCommand();
            posUpdate.Command = Command.PositionUpdate;
            posUpdate.Message = m_game.GetPositions();
            // Convert to a byte array and send it to everyone.
            byte[] posBytes = GetBytes(posUpdate);
            // Apparently a sleep is necessary to prevent the client programs from blowing up... let's just make it quick.
            System.Threading.Thread.Sleep(50);
            SendToAll(posBytes);
        }
        // Sends the amount of funds the player currently has to the player.
        private void SendPlayerFunds(string a_name)
        {
            // Get the player.
            Player requester = m_game.GetPlayer(a_name);
            // Get their funds.
            int funds = requester.Funds;
            // Send it over.
            PlayerCommand fundSend = new PlayerCommand();
            fundSend.Command = Command.FundUpdate;
            fundSend.Message = funds.ToString();
            byte[] fundBytes = GetBytes(fundSend);
            m_clientSockets[requester.RosterPosition].BeginSend(fundBytes, 0, fundBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendPlayerNames()

        NAME

                SendPlayerNames() - sends a list of names to a player

        SYNOPSIS

                private void SendPlayerNames(string a_name)

                a_name -> the player requesting the list of names of other players

        DESCRIPTION

                This function sends a list of players currently still in the game to
                a player that requests them. It does this by getting the list of all
                the players then going through them all and adding them to a string,
                separating them by commas. The foreach loop makes sure to exclude the
                player requesting the list. After all is said and done, it sends the
                list over to the player using the GetNames command.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                11:02am 5/15/2017

        */
        /**/
        private void SendPlayerNames(string a_name)
        {
            // Get the player requesting the list.
            Player requester = m_game.GetPlayer(a_name);
            // Get a list of the players.
            List<string> playerList = m_game.GetPlayers();
            // Start a string list to send over.
            string sendingList = "";
            foreach (string name in playerList)
            {
                // If the name is not equal to a_name (the player requesting the list), add it to the string.
                if (name != a_name)
                {
                    // Append a beginning bracket, the name, and an end bracket.
                    sendingList = sendingList + name + ",";
                }
            }
            // Send over the list.
            PlayerCommand listSend = new PlayerCommand();
            listSend.Command = Command.GetNames;
            listSend.Message = sendingList;
            byte[] listBytes = GetBytes(listSend);
            m_clientSockets[requester.RosterPosition].BeginSend(listBytes, 0, listBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendProperties()

        NAME

                SendProperties() - sends a list of properties a player has to another player

        SYNOPSIS

                private void SendProperties(string a_requester, string a_requestedPlayer)

                a_requester -> the player requesting the list of properties
                a_requestedPlayer -> the player whose property list is requested

        DESCRIPTION

                This function sends a list of properties a player owns to another player
                for a prospective trade. This is done by looking at the list of properties
                the requested player owns and adds each property to the list, but only if the
                property is not mortgaged and has no buildings on it. Afterwards, a comma
                separated string list is built based on the list and sent over to the requester. 

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                8:52pm 5/30/2017

        */
        /**/
        private void SendProperties(string a_requester, string a_requestedPlayer)
        {
            // Get the respective players.
            Player requester = m_game.GetPlayer(a_requester);
            Player requestedPlayer = m_game.GetPlayer(a_requestedPlayer);
            // Look at the player's properties.
            ArrayList properties = requestedPlayer.Properties;
            // Create a new list of eligible properties to send.
            List<string> propertiesToSend = new List<string>();
            foreach (Property property in properties)
            {
                // If this property is not mortgaged, keep checking.
                if (!property.IsMortgaged)
                {
                    // Do another check to see if all of the properties of that building type have no houses.
                    // If so, add it to the list. Otherwise, don't.
                    if (m_game.AllPropertiesHaveNoHouses(property.Color))
                    {
                        propertiesToSend.Add(property.Name);
                    }
                }
            }
            // Start sending these properties over.
            PlayerCommand propertySender = new PlayerCommand();
            propertySender.Command = Command.GetProperties;
            string propList = "";
            foreach (string property in propertiesToSend)
            {
                propList = propList + property + ",";
            }
            propertySender.Message = propList;
            byte[] propertyByte = GetBytes(propertySender);
            m_clientSockets[requester.RosterPosition].BeginSend(propertyByte, 0, propertyByte.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendTradeFunds()

        NAME

                SendTradeFunds() - sends a list of properties a player has to another player

        SYNOPSIS

                private void SendTradeFunds(string a_requester, string a_requestedPlayer)

                a_requester -> the player requesting the funds of each player
                a_requestedPlayer -> the player whose funds are requested

        DESCRIPTION

                This function sends the funds of a requesting player and a requested player to
                the requester for a prospective trade. It does this by getting both of their funds,
                transforming them into strings, separating those strings in another string with a comma,
                then sending it over to the requester using the GetFunds command. 

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                9:06pm 5/30/2017

        */
        /**/
        private void SendTradeFunds(string a_requester, string a_requestedPlayer)
        {
            // Get the respective players.
            Player requester = m_game.GetPlayer(a_requester);
            Player requestedPlayer = m_game.GetPlayer(a_requestedPlayer);
            // Get both of their funds.
            string requesterFunds = requester.Funds.ToString();
            string requestedPlayerFunds = requestedPlayer.Funds.ToString();
            // Separate with a comma and send over.
            string bothFunds = requesterFunds + "," + requestedPlayerFunds;
            PlayerCommand fundSender = new PlayerCommand();
            fundSender.Command = Command.GetFunds;
            fundSender.Message = bothFunds;
            byte[] fundBytes = GetBytes(fundSender);
            m_clientSockets[requester.RosterPosition].BeginSend(fundBytes, 0, fundBytes.Length, SocketFlags.None,
                 new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        DoTradeRequest()

        NAME

                DoTradeRequest() - sends a request for a trade to a player

        SYNOPSIS

                private void DoTradeRequest(string a_name, string a_message)

                a_name -> the name of the player who is requesting the trade
                a_message -> the message containing all the information about the trade

        DESCRIPTION

                This function sends a request to trade to a specific player. This player
                is received through parsing a_message and accessing the appropriate part
                of the message to receive the player (always split as property,money,player,
                property,money). Then it sends the info about the trade, a_message, to the
                player who is being requested for a trade using the TradeRequest command.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                9:42pm 5/30/2017

        */
        /**/
        private void DoTradeRequest(string a_name, string a_message)
        {
            // Parse some necessary info:
            string[] tradeInfo = a_message.Split(',');
            // Get the player that the request will be sent to.
            Player requestedPlayer = m_game.GetPlayer(tradeInfo[2]);
            // Send the info just received to this player.
            PlayerCommand requestSender = new PlayerCommand();
            requestSender.Name = a_name;
            requestSender.Command = Command.TradeRequest;
            requestSender.Message = a_message;
            byte[] tradeBytes = GetBytes(requestSender);
            m_clientSockets[requestedPlayer.RosterPosition].BeginSend(tradeBytes, 0, tradeBytes.Length,
                SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        ProcessTrade()

        NAME

                ProcessTrade() - sends a request for a trade to a player

        SYNOPSIS

                private void ProcessTrade(string a_name, string a_tradeInfo)

                a_name -> the name of the player who is requesting the trade
                a_tradeInfo -> the information about the trade that will be done

        DESCRIPTION

                This function processes a trade that has been approved by both players.
                It parses the tradeInfo into an array of strings then begins the trade.
                It only performs certain parts of the trade if those parts do not have
                "null" as their part of the trade information. Once the trade is complete,
                it sends a message to both players indicating that the trade was a success.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                10:46pm 5/30/2017

        */
        /**/
        private void ProcessTrade(string a_name, string a_tradeInfo)
        {
            // Parse the trade info.
            string[] tradeInfo = a_tradeInfo.Split(',');
            // Get players.
            Player requester = m_game.GetPlayer(tradeInfo[2]);
            Player requestedPlayer = m_game.GetPlayer(a_name);
            // Trade the original offered property, if there is one.
            if (tradeInfo[0] != "null")
            {
                // Take this property out of the requester's property list then add it to the requested player's.
                m_game.TradeProperty(ref requester, ref requestedPlayer, tradeInfo[0]);
            }
            // Transfer money if necessary.
            if (tradeInfo[1] != "null")
            {
                m_game.PayUp(Int32.Parse(tradeInfo[1]), ref requester, ref requestedPlayer);
            }
            // Trade the requested property, if there is one.
            if (tradeInfo[3] != "null")
            {
                // Take this property out of the requested player's property list then add it to the requester's.
                m_game.TradeProperty(ref requestedPlayer, ref requester, tradeInfo[3]);
            }
            // Transfer money if necessary.
            if (tradeInfo[4] != "null")
            {
                m_game.PayUp(Int32.Parse(tradeInfo[4]), ref requestedPlayer, ref requester);
            }

            // Trading is done! Send an AcceptTrade command to both players to indicate success and perform GUI changes on their end.
            PlayerCommand acceptSender = new PlayerCommand();
            acceptSender.Command = Command.AcceptTrade;
            // First, the requester:
            string tradedProperties = tradeInfo[0] + "," + tradeInfo[3];
            acceptSender.Message = tradedProperties;
            byte[] requesterBytes = GetBytes(acceptSender);
            m_clientSockets[requester.RosterPosition].BeginSend(requesterBytes, 0, requesterBytes.Length,
                SocketFlags.None, new AsyncCallback(SendCallback), null);
            // And now the requested:
            tradedProperties = tradeInfo[3] + "," + tradeInfo[0];
            acceptSender.Message = tradedProperties;
            byte[] requestedPlayerBytes = GetBytes(acceptSender);
            m_clientSockets[requestedPlayer.RosterPosition].BeginSend(requestedPlayerBytes, 0, requestedPlayerBytes.Length,
                SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        // Informs the player requesting a trade that their request was denied.
        private void DeclinedTrade(string a_name)
        {
            // Get the player who initially offered.
            Player requester = m_game.GetPlayer(a_name);
            // Send a DeclineTrade command to them.
            PlayerCommand declineSender = new PlayerCommand();
            declineSender.Command = Command.DeclineTrade;
            byte[] declineBytes = GetBytes(declineSender);
            m_clientSockets[requester.RosterPosition].BeginSend(declineBytes, 0, declineBytes.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendPropertiesToBuildOn()

        NAME

                SendPropertiesToBuildOn() - sends a request for a trade to a player

        SYNOPSIS

                private void SendPropertiesToBuildOn(string a_name)

                a_name -> the name of the player who is trying to buy buildings

        DESCRIPTION

                This function sends a list of properties that are able to be
                built on to a player requesting the list. It does this by getting
                the list of properties that can be built on in Game's
                GetPropertiesToBuildOn function and builds a comma separated string
                to send over to the player. It then sends that message over with the
                GetBuildingProperties command.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                1:54pm 5/31/2017

        */
        /**/
        private void SendPropertiesToBuildOn(string a_name)
        {
            // Get the list of properties.
            List<string> propertyList = m_game.GetPropertiesToBuildOn(a_name);
            // Build a comma separated string and then send it to the client requesting the list.
            string commaSeparatedProperties = "";
            foreach (string property in propertyList)
            {
                commaSeparatedProperties = commaSeparatedProperties + property + ",";
            }
            PlayerCommand propertySender = new PlayerCommand();
            propertySender.Command = Command.GetBuildingProperties;
            propertySender.Message = commaSeparatedProperties;
            byte[] propertyBytes = GetBytes(propertySender);
            m_clientSockets[m_game.GetPlayer(a_name).RosterPosition].BeginSend(propertyBytes, 0, propertyBytes.Length,
                SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        SendBuildingInfo()

        NAME

                SendBuildingInfo() - sends a request for a trade to a player

        SYNOPSIS

                private void SendBuildingInfo(string a_name, string a_propertyName)

                a_name -> the name of the player who owns the property and wants to get building info
                a_propetyName -> the name of the property to get the building info for

        DESCRIPTION

                This function sends the information about a property's buildings to a player who wants
                to know said information. It gets the amount of buildings on the property's space and
                the cost of buildings to put on it. It then converts these numbers to strings and sends
                them to the player in a comma separated string with the GetBuildingInfo command.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                2:17pm 5/31/2017

        */
        /**/
        private void SendBuildingInfo(string a_name, string a_propertyName)
        {
            // Get the property and the space that contains the propery.
            Space propertySpace = m_game.GetSpaceWithProperty(a_propertyName);
            Property property = propertySpace.GetProperty();
            // Get the amount of buildings on the space and the cost of buildings for the property, then send the info over.
            int buildingAmount = propertySpace.BuildingAmount;
            int buildingCost = property.BuildingCost;
            // Build a separated list and send it over.
            string numbersList = buildingAmount.ToString() + "," + buildingCost.ToString();
            PlayerCommand buildingSender = new PlayerCommand();
            buildingSender.Command = Command.GetBuildingInfo;
            buildingSender.Message = numbersList;
            byte[] buildingBytes = GetBytes(buildingSender);
            m_clientSockets[m_game.GetPlayer(a_name).RosterPosition].BeginSend(buildingBytes, 0, buildingBytes.Length,
                SocketFlags.None, new AsyncCallback(SendCallback), null);
        }
        /**/
        /*
        BuyBuildingActions()

        NAME

                BuyBuildingActions() - performs actions for buying a building

        SYNOPSIS

                private void BuyBuildingActions(string a_name, string a_propertyName)

                a_name -> the name of the player who wants to buy a building
                a_propetyName -> the name of the property to build on

        DESCRIPTION

                This function builds on a property. Before doing anything, it determines if
                the player has sufficient funds for a building. If they don't, it sends back
                the BuyBuilding command to indicate that a building cannot be bought for the
                property/space due to not having enough money. Otherwise, it adds a building,
                subtracts the cost of the building from the player's funds, and sends a fund
                update to the player.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                2:43pm 5/31/2017

        */
        /**/
        private void BuyBuildingActions(string a_name, string a_propertyName)
        {
            // Get the player.
            Player player = m_game.GetPlayer(a_name);
            // Get the space.
            Space space = m_game.GetSpaceWithProperty(a_propertyName);
            // First, determine if the player has sufficient funds for a building.
            if (player.Funds < space.GetProperty().BuildingCost)
            {
                // Send a BuyBuilding command back to indicate you cannot build a building because of funds.
                PlayerCommand noBuySender = new PlayerCommand();
                noBuySender.Command = Command.BuyBuilding;
                noBuySender.Message = a_propertyName;
                byte[] noBuyBytes = GetBytes(noBuySender);
                m_clientSockets[player.RosterPosition].BeginSend(noBuyBytes, 0, noBuyBytes.Length, SocketFlags.None,
                    new AsyncCallback(SendCallback), null);
            }
            else
            {
                // Add a building to this space and subtract the building cost.
                space.AddBuilding();
                player.Funds = player.Funds - space.GetProperty().BuildingCost;
                // Send a fund update.
                SendPlayerFunds(a_name);
            }

        }
        // Sells a building on the property requested, then sends a fund update.
        private void SellBuildingActions(string a_name, string a_propertyName)
        {
            // Get the player.
            Player player = m_game.GetPlayer(a_name);
            // Get the space.
            Space space = m_game.GetSpaceWithProperty(a_propertyName);
            // No checks necessary, just remove the building and add funds.
            space.RemoveBuilding();
            player.Funds = player.Funds + (space.GetProperty().BuildingCost / 2);
            // Send a fund update.
            SendPlayerFunds(a_name);
        }
        /**/
        /*
        BankruptPlayerActions()

        NAME

                BankruptPlayerActions() - performs actions for bankrupting a player

        SYNOPSIS

                private void BankruptPlayerActions(string a_name)

                a_name -> the name of the player to bankrupt

        DESCRIPTION

                This function performs appropriate actions on a player that is declaring bankruptcy. It
                calls LiquidateAndBankrupt on them to get rid of any assets they may have and move them
                into the bankrupt player roster. Then it sees if there is only one remaining player left
                in the game. If so, they win! Everyone is sent a message about the winner of the game.
                It is now over.

        RETURNS

                Nothing!

        AUTHOR

                Bryan Leier

        DATE

                10:34pm 5/31/2017

        */
        /**/
        private void BankruptPlayerActions(string a_name)
        {
            // This player is declaring bankruptcy. Get rid of any assets they may have.
            Player bankruptPlayer = m_game.GetPlayer(a_name);
            m_game.LiquidateAndBankrupt(ref bankruptPlayer);
            // Now check the game's player roster. Is there only 1 player left?
            List<string> remainingPlayers = m_game.GetPlayers();
            if (remainingPlayers.Count == 1)
            {
                // All other players are bankrupt. This player has won the game!
                // Send this message to everyone.
                PlayerCommand winSend = new PlayerCommand();
                winSend.Name = remainingPlayers[0];
                winSend.Command = Command.Winner;
                byte[] winBytes = GetBytes(winSend);
                foreach(Socket socket in m_clientSockets)
                {
                    socket.BeginSend(winBytes, 0, winBytes.Length, SocketFlags.None, new AsyncCallback(SendCallback),
                        null);
                }
            }
        }

    	// The server's socket.
    	private Socket m_serverSocket;
    	// The list of clients.
    	private List<Socket> m_clientSockets;
    	// The buffer that the server will use for communication between clients.
    	private byte[] m_buffer = new byte[1024];
    	// The game to play on the server.
    	private Game m_game;
        // Boolean that determines whether or not the game has started.
        private bool m_gameStarted;
    }
}