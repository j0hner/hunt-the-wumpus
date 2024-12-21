using System.Runtime.CompilerServices;
using Custom.InputHandling;
using System.Linq;

internal class Program
{
    static string RootPath = AppContext.BaseDirectory;
    static string TextPath = RootPath + "Assets/Text.txt";
    static string GraphicsPath = RootPath + "Assets/graphics.txt";
    static string InstructionsPath = RootPath + "Assets/instructions.txt";
    static InputReader<string> stringReader = new InputReader<string>();
    static InputReader<int> intReader = new InputReader<int>();
    static Random RandomGen = new Random();
    static Room[] Rooms = new Room[20];
    static string G_Arrow = "", G_Grave = "", G_Room = "", G_RoomInverted = "", G_Debug = "", G_Textbox1 = "", G_Textbox2 = "", G_Textbox3 = "", G_IntroText1 = "", G_IntroText2 = "", G_IntroText3 = "", G_Bats = "", G_WumpDeath = "", G_Win = "", G_Miss = "", G_Pit = "";
    static List<string> LoseText = [];
    static List<string> FillerText = [];
    static int[] currentRoomConnections = [];
    

    public static void Main()
    {
        Console.Title = "Hunt The Wumpus";
        
        LoadGraphics();

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        #region intro
        int sleepTime = 1500;
        Console.WriteLine(G_IntroText1);
        Thread.Sleep(sleepTime);
        Console.Clear();
        Console.WriteLine(G_IntroText2);
        Thread.Sleep(sleepTime);
        Console.Clear();
        Console.WriteLine(G_IntroText3);
        Thread.Sleep(sleepTime);
        Console.Clear();
        #endregion
        
        Game();
    }

    static void Game()
    {
        bool IsMovement = true;
        
        Console.Clear();
        
        #region roomSetup
        Rooms[0] = new Room([5, 6, 2], true);       //1
        Rooms[1] = new Room([1, 8, 3], true);       //2
        Rooms[2] = new Room([2, 10, 4], true);      //3
        Rooms[3] = new Room([3, 12, 5], true);      //4
        Rooms[4] = new Room([4, 14, 1], true);      //5
        Rooms[5] = new Room([15, 1, 7], false);     //6
        Rooms[6] = new Room([6, 17, 8], true);      //7
        Rooms[7] = new Room([7, 2, 9], false);      //8
        Rooms[8] = new Room([8, 18, 10], true);     //9
        Rooms[9] = new Room([9, 3, 11], false);     //10
        Rooms[10] = new Room([10, 19, 12], true);   //11
        Rooms[11] = new Room([11, 4, 13], false);   //12
        Rooms[12] = new Room([12, 20, 14], true);   //13
        Rooms[13] = new Room([13, 5, 15], false);   //14
        Rooms[14] = new Room([14, 16, 6], true);    //15
        Rooms[15] = new Room([20, 15, 17], false);  //16
        Rooms[16] = new Room([16, 7, 18], false);   //17
        Rooms[17] = new Room([17, 9, 19], false);   //18
        Rooms[18] = new Room([18, 11, 20], false);  //19
        Rooms[19] = new Room([19, 13, 16], false);  //20
        #endregion

        Console.CursorVisible = true;
        while (true)
        {
            if (stringReader.GetInputFromList("Instructions? (y/n):", out string choice, ["y", "n"]) && choice == "y")
            {
                Console.WriteLine(File.ReadAllText(InstructionsPath));
                Console.ReadLine();
                break;
            }
            else if (choice == "n") break;
        }
        
        foreach(Room room in Rooms)
        {
            room.HasStench = false;
            room.HasWumpus = false;
            room.HasFlapping = false;
            room.HasBats = false;
            room.HasBreeze = false;
            room.HasPit = false;
        }
        
        PlaceHazards(2, 2, true);
        
        int startRoomId;
        do startRoomId = RandomGen.Next(20); while (Rooms[startRoomId].HasBats || Rooms[startRoomId].HasPit || Rooms[startRoomId].HasWumpus || Rooms[startRoomId].HasBreeze || Rooms[startRoomId].HasStench || Rooms[startRoomId].HasFlapping);
        
        Rooms[startRoomId].Enter();

        while (true)
        {
            Console.CursorVisible = true;

            List<string> tempConnections = [];
            foreach(int i in currentRoomConnections)
            {
                tempConnections.Add($"{i}");
            }
            
            tempConnections.Add(IsMovement ? "s" : "m");
            
            var options = tempConnections.ToArray();

            if (stringReader.GetInputFromList(IsMovement ? "Move> " : "Shoot> ", out string choice, options))
            {
                if (int.TryParse(choice, out int choiceInt))
                {
                    if (IsMovement) 
                    {
                        Rooms[choiceInt - 1].Enter();
                    }
                    else 
                    {
                        Rooms[choiceInt - 1].Shoot();
                    }
                }
                else
                {
                    IsMovement = !IsMovement;
                }
            }
        }
    }

    static void LoadGraphics()
    {
        string[] AllGraphics = File.ReadAllText(GraphicsPath).Split(';');
        G_Debug = AllGraphics[0];
        G_Room = AllGraphics[1];
        G_RoomInverted = AllGraphics[2];
        G_Grave = AllGraphics[3];
        G_Arrow = AllGraphics[4];
        G_Textbox1 = AllGraphics[5];
        G_Textbox2 = AllGraphics[6];
        G_Textbox3 = AllGraphics[7];
        G_IntroText1 = AllGraphics[8];
        G_IntroText2 = AllGraphics[9];
        G_IntroText3 = AllGraphics[10];
        G_Bats = AllGraphics[11];
        G_WumpDeath = AllGraphics[12];
        G_Win = AllGraphics[13];
        G_Miss = AllGraphics[14];
        G_Pit = AllGraphics[15];
        string[] AllText = File.ReadAllLines(TextPath);
        bool isLosetext = true;

        foreach (string line in AllText)
        {
            if (line == ";")
            {
                isLosetext = false;
                continue;
            }

            if (isLosetext) LoseText.Add(line);
            else FillerText.Add(line);
        }
    }

    static void PlaceHazards(int pitCount, int batCount, bool placeWump)
    {

        for (int i = 0; i < pitCount; i++) //place pits
        {
            int ChosenRoomId;
            do ChosenRoomId = RandomGen.Next(20); while (Rooms[ChosenRoomId].HasPit);
            Rooms[ChosenRoomId].HasPit = true;

            foreach (int j in Rooms[ChosenRoomId].Connections)
            {
                Rooms[j - 1].HasBreeze = true;
            }
        }

        for (int i = 0; i < batCount; i++) //place Bats
        {
            int ChosenRoomId;
            do ChosenRoomId = RandomGen.Next(20); while (Rooms[ChosenRoomId].HasBats);
            Rooms[ChosenRoomId].HasBats = true;

            foreach (int j in Rooms[ChosenRoomId].Connections)
            {
                Rooms[j - 1].HasFlapping = true;
            }
        }

        if (placeWump)
        {
            int wumpusRoom = RandomGen.Next(20);
            Rooms[wumpusRoom].HasWumpus = true;
            foreach (int j in Rooms[wumpusRoom].Connections)
            {
                Rooms[j - 1].HasStench = true;
            }
        }
    }

    static void Loss(Death cause)
    {
        Console.Clear();

        switch (cause)
        {
            case Death.ByWumpus:
                Console.Write(G_WumpDeath);
                break;
            case Death.ByPit:
                Console.Write(G_Pit);
                break;
            case Death.ByMiss:
                Console.Write(G_Miss);
                break;
        }

        Thread.Sleep(2000);
        Console.Clear();
        Console.Write(G_Grave);
        Console.WriteLine(G_Textbox1);
        Console.SetCursorPosition(2, 14);
        Console.Write(LoseText[RandomGen.Next(LoseText.Count)]);

        Console.SetCursorPosition(0, 17);
        Console.Write("Enter to restart");
        Console.ReadLine();
        Game();
    }

    static void Bats()
    {
        int RoomId;
        do RoomId = RandomGen.Next(20); while (Rooms[RoomId].HasBats || Rooms[RoomId].HasWumpus);
        Console.Clear();
        Console.WriteLine(G_Bats);
        Thread.Sleep(2500);
        PlaceHazards(0, 1, false);
        Rooms[RoomId].Enter();
    }

    static void Win()
    {
        Console.Clear();
        Console.WriteLine($"{G_Win}\nEnter to restart");
        Console.ReadLine();
        Game();
    }
    
    class Room(int[] connections, bool inverted)
    {
        public bool HasWumpus { get; set; } = false;
        public bool HasPit { get; set; } = false;
        public bool HasBats { get; set; } = false;
        public bool HasStench { get; set; } = false;
        public bool HasBreeze { get; set; } = false;
        public bool HasFlapping { get; set; } = false;
        public int[] Connections { get; } = connections;
        bool IsInverted = inverted;

        public void Enter()
        {
            static string FormatNumber(int num) => (num.ToString().Length == 1) ? num + " " : num.ToString();

            currentRoomConnections = Connections;

            Console.CursorVisible = false;

            Console.Clear();

            if (HasWumpus) Loss(Death.ByWumpus);
            else if (HasBats) 
            {
                Bats();
                return;
            }
            else if (HasPit) Loss(Death.ByPit);

            if (IsInverted)
            {
                Console.WriteLine(G_RoomInverted);
                Console.SetCursorPosition(15, 3);
                Console.Write(FormatNumber(Connections[1]));
            }
            else
            {
                Console.WriteLine(G_Room);
                Console.SetCursorPosition(15, 9);
                Console.Write(FormatNumber(Connections[1]));
            }

            Console.SetCursorPosition(21, 6);
            Console.Write(FormatNumber(Connections[2]));
            Console.SetCursorPosition(9, 6);
            Console.Write(FormatNumber(Connections[0]));

            Console.SetCursorPosition(0, 13);
            int boxSize = 0;

            if (HasBreeze) boxSize++;
            if (HasFlapping) boxSize++;
            if (HasStench) boxSize++;

            switch (boxSize)
            {
                case 0:
                    Console.Write(G_Textbox1);
                    Console.SetCursorPosition(2, 14);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(FillerText[RandomGen.Next(FillerText.Count)]);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 17);
                    break;
                case 1:
                    Console.Write(G_Textbox1);
                    Console.SetCursorPosition(2, 14);
                    if (HasBreeze) Console.Write("I feel a breeze");
                    else if (HasStench) Console.Write("I smell a terrible stench");
                    else if (HasFlapping) Console.Write("I hear wings flaping");
                    Console.SetCursorPosition(0, 17);
                    break;
                case 2:
                    Console.Write(G_Textbox2);
                    Console.SetCursorPosition(2, 14);
                    if (HasBreeze)
                    {
                        Console.Write("I feel a breeze");
                        Console.SetCursorPosition(2, 15);
                    }
                    if (HasStench)
                    {
                        Console.Write("I smell a terrible stench");
                        Console.SetCursorPosition(2, 15);
                    }
                    if (HasFlapping) Console.Write("I hear wings flaping");
                    Console.SetCursorPosition(0, 18);
                    break;
                case 3:
                    Console.Write(G_Textbox3);
                    Console.SetCursorPosition(2, 14);
                    Console.Write("I feel a breeze");
                    Console.SetCursorPosition(2, 15);
                    Console.Write("I smell a terrible stench");
                    Console.SetCursorPosition(2, 16);
                    if (HasFlapping) Console.Write("I hear wings flaping");
                    Console.SetCursorPosition(0, 19);
                    break;
            }

            
        }

        public void Shoot()
        {
            Console.Clear();
            Console.WriteLine(G_Arrow);
            Thread.Sleep(3000);
            
            if (HasWumpus) Win();
            else Loss(Death.ByMiss);
        }
    }

    enum Death
    {
        ByWumpus,
        ByPit,
        ByMiss
    }
}
