using System;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Xml.Serialization;
using System.Reflection;


// Abstrakte Basisklasse für alle Maze-Elemente
abstract class MazeElement
{
    public int ID { get; private set; }
    public List<DockingPoint> DockingPoints { get; } = new List<DockingPoint>();

    protected MazeElement(int id)
    {
        ID = id;
    }
}

class Corridor : MazeElement
{
    
    public Corridor(int id) : base(id)
    {
        // 4 DockingPoints: 2 für Chambers, 2 für Intersections
        for (int i = 0; i < 4; i++)
            DockingPoints.Add(new DockingPoint(this));
    }
    

}

class Chamber : MazeElement
{
    //public DockingPoint DockingPoint { get; }
    public string Content { get; private set; }

    public Chamber(int id, string content) : base(id)
    {
        //DockingPoint = new DockingPoint(this);
        DockingPoints.Add(new DockingPoint(this));
        Content = content;
    }

    public bool hasContent()
    {
        if (Content != null && !Content.Equals(""))
        {
            return true;
        }
        return false;
    }
}

class TieIntersection : MazeElement
{
    public TieIntersection(int id) : base(id)
    {
        for (int i = 0; i < 3; i++)
            DockingPoints.Add(new DockingPoint(this));
    }
}

class CrossIntersection : MazeElement
{
    public CrossIntersection(int id) : base(id)
    {
        for (int i = 0; i < 4; i++)
            DockingPoints.Add(new DockingPoint(this));
    }
}

// Verbindung zwischen zwei DockingPoints
class Connector
{
    public int ID { get; private set; }
    public DockingPoint PointA { get; private set; }
    public DockingPoint PointB { get; private set; }

    public Connector(int id, DockingPoint pointA, DockingPoint pointB)
    {
        ID = id;
        PointA = pointA;
        PointB = pointB;
        pointA.Connect(this);
        pointB.Connect(this);
    }
}

// DockingPoint als Verbindungspunkt
class DockingPoint
{
    public MazeElement Parent { get; }
    public Connector ConnectedTo { get; private set; }

    public DockingPoint(MazeElement parent)
    {
        Parent = parent;
    }

    public void Connect(Connector connector)
    {
        ConnectedTo = connector;
    }
}

// Spieler und Rucksack
class Player
{
    public List<string> Inventory { get; } = new List<string>();
    //public List<string> SolvedActions { get; } = new List<string>();
    public DockingPoint CurrentDockingPoint { get; set; }

    public Player(DockingPoint startPoint)
    {
        CurrentDockingPoint = startPoint;
    }
}

// Repository zur Verwaltung aller Objekte
class Repository
{
    public List<MazeElement> MazeElements { get; } = new List<MazeElement>();
    public List<Connector> Connectors { get; } = new List<Connector>();
}

class Riddle
{

    public string Finding { get; private set; }
    public string Action { get; private set;  }
    public bool IsDiscoveredFinding { get; private set; }
    public bool IsDiscoveredAction { get; private set; }
    public bool IsSolved => IsDiscoveredFinding && IsDiscoveredAction;
    public string Winny { get; private set; }
    public string Loosy { get; private set; }

    public Riddle(string finding, string action, string winny, string loosy)
    {
        Finding = finding;
        Action = action;
        IsDiscoveredFinding = false;
        IsDiscoveredAction = false;
        Winny = winny;
        Loosy = loosy;
    }

    public void Discover(string input)
    {
        if (input == Finding)
            IsDiscoveredFinding = true;
        if (input == Action)
            IsDiscoveredAction = true;
    }

    public bool TrySolve(string input1, string input2)
    {
        return (input1 == Finding && input2 == Action) || (input1 == Action && input2 == Finding);
    }
}


class RiddlesRepository
{
    private static List<Riddle> riddles;
    private static Random random;
    private static bool allSolved = false;

    public static void initRiddlesRepository()
    {
        riddles = new List<Riddle>
        {
            new Riddle("Schlüssel", "Truhe", "Die Truhe ist offen aber leer. Trotzdem, gut gemacht!", "So wird die Truhe nicht aufgehen. Quote Trump: You're so sad!"),
            new Riddle("Knoblauch", "Vampir", "Der Vampir ist abgewehrt. Gut gemacht!", "So wird dich der Vampir holen. Quote Trump: You're a looser!"),
            new Riddle("Rum", "Durstiger Pirat", "Der Pirat ist zufrieden. Gut gemacht!", "Jetzt ist der Pirat durstig UND wütend. Quote Trump: You're so lousy!"),
            new Riddle("Ring", "Frodo", "Du hast den Test bestanden und Frodo seinen Ring zurückgegeben. Gut gemacht!", "Frodo fühlt sich beleidigt. Quote Trump: You're so nasty!")
        };
        random = new Random();
    }

    public static bool ShallUseFinding()
    {
        return random.Next(2) == 0; // 50% Chance für Finding oder Action
    }

    public static string GetUndiscoveredFinding()
    {
        var undiscoveredFindings = riddles.Where(r => !r.IsDiscoveredFinding).Select(r => r.Finding).ToList();
        return undiscoveredFindings.Count > 0 ? undiscoveredFindings[random.Next(undiscoveredFindings.Count)] : string.Empty;
    }

    public static string GetUndiscoveredAction()
    {
        var undiscoveredActions = riddles.Where(r => !r.IsDiscoveredAction).Select(r => r.Action).ToList();
        return undiscoveredActions.Count > 0 ? undiscoveredActions[random.Next(undiscoveredActions.Count)] : string.Empty;
    }

    public static bool IsStringFinding(string input)
    {
        return riddles.Any(r => r.Finding == input);
    }

    public static bool IsStringAction(string input)
    {
        return riddles.Any(r => r.Action == input);
    }

    public static bool HasUnsolvedRiddles()
    {
        return riddles.Any(r => !r.IsSolved);
    }

    public static int AmountOfSolvedRiddles()
    {
        return riddles.Count(r => r.IsSolved);
    }

    public static int AmountOfUnsolvedRiddles()
    {
        return riddles.Count(r => !r.IsSolved);
    }

    public static void ResultOfAction(string action, string finding)
    {
        var solvedRiddle = riddles.FirstOrDefault(r => r.TrySolve(action, finding));

        if (solvedRiddle != null)
        {
            if (solvedRiddle.IsSolved)
                Console.WriteLine($"Das Rätsel mit {action} und {finding} wurde bereits gelöst!");
            else
            {
                solvedRiddle.Discover(action);
                solvedRiddle.Discover(finding);
                Console.WriteLine(solvedRiddle.Winny);
                Console.WriteLine($"Glückwunsch! Du hast das Rätsel mit {action} und {finding} gelöst! Quote Trump: You did a tremendous job!");

                if(!allSolved && RiddlesRepository.AmountOfUnsolvedRiddles() == 0)
                {
                    Console.WriteLine("Du hast alle Rätsel gelöst, das ist ja super! Quote Trump: He/She's a great person, I don't know him/her, but I've heard he/she was fantastic!");
                }
            }
        } else
        {
            var unsolvedRiddel = FindRiddleByAction(action);
            if (unsolvedRiddel != null)
            {
                Console.WriteLine(unsolvedRiddel.Loosy);
                Console.WriteLine($"Die Kombination {action} und {finding} ergibt kein gültiges Rätsel.");
            }
        }
    }
    public static Riddle FindRiddleByAction(string action)
    {
        return riddles.FirstOrDefault(r => r.Action == action);
    }

    public static string generateContent()
    {
        if(random.Next(3) == 0)
        {
            if (ShallUseFinding())
            {
                return GetUndiscoveredFinding();
            }
            else
            {
                return GetUndiscoveredAction();
            }
        }

        return "";
    }
}




class MagicMaze
{
    private static Random random = new Random();
    private static int nextMazeElementID = 1;
    private static int nextConnectorID = 1;
    

    public static MazeElement CreateMazeElement(string type, string content = "")
    {
        int id = nextMazeElementID++;
        //return type switch
        MazeElement newElement = type switch
        {
            "Corridor" => new Corridor(id),
            "Chamber" => new Chamber(id, content),
            "TieIntersection" => new TieIntersection(id),
            "CrossIntersection" => new CrossIntersection(id),
            _ => throw new ArgumentException("Invalid MazeElement type")
        };

        return newElement;
    }

    public static Connector CreateConnector(DockingPoint pointA, DockingPoint pointB)
    {
        int id = nextConnectorID++;
        return new Connector(id, pointA, pointB);
    }
}

class Program
{
    private static Repository repository = new Repository();
    private static Random random = new Random();
    private static Player? player;
    private static bool gameRunning;
    private static MazeElement? currentElement;
    


    static void Main()
    {
        initGame();
        Console.WriteLine("Willkommen im Magic Maze!");
        gameRunning = true;

        while (gameRunning)
        {
            if (player != null)
            {
                currentElement = player.CurrentDockingPoint.Parent;
            }

            //Momentan gibt es nur Korridor-Aktionen (MOVE), es muss aber auch Raum-Aktionen (ACTION) geben. Wie werden die unterschieden?
            if (currentElement is Chamber)
            {
                //Der Spieler befindet sich in einem Raum
                //Im Raum gibt es nur ACTION oder die Möglichkeit raus zu gehen.
                conductChamberACTION((Chamber)currentElement);
            }
            else
            {
                //Der Spieler befindet sich in einem Korridor oder auf einer Kreuzung
                //Hier gibt es nur die Möglichkeit auf einen anderen Verbindungspunkt zu wechseln (MOVE).
                if (currentElement != null)
                {
                    conductMOVE(currentElement);
                }
            }
        }
    }

    private static void conductChamberACTION(Chamber chamber)
    {
        Console.WriteLine($"Du befindest dich in einem Raum (ID: {chamber.ID}).");
        
        var dp = chamber.DockingPoints[0];
        var target = dp.ConnectedTo?.PointA == dp ? dp.ConnectedTo.PointB.Parent : dp.ConnectedTo?.PointA.Parent;
        if (target == null)
        {
            Console.WriteLine("ERROR: conductChamberACTION(): Das target in diesem Raum darf nicht null sein.");
            return;
        }

        var chamberHasContent = false;
        var chamberHasFinding = false;
        var riddleIsSolved = true;
        //var riddle = null;

        if (chamber.hasContent())
        {
            chamberHasContent = true;
            if (RiddlesRepository.IsStringFinding(chamber.Content))
            {
                chamberHasFinding = true;
                Console.WriteLine($"Im Raum findest du: {chamber.Content}");
                Console.WriteLine("Was möchtest du tun?");
                Console.WriteLine($"1: Gehe zu {target.GetType().Name} (ID: {target.ID})");
                Console.WriteLine($"2: '{chamber.Content}' in den Rucksack nehmen.");
            }   
            else if (RiddlesRepository.IsStringAction(chamber.Content))
            {
                var riddle = RiddlesRepository.FindRiddleByAction(chamber.Content);

                if (riddle == null)
                {
                    Console.WriteLine($"ERROR: conductChamberACTION(): Kein Rätsel gefunden für: {chamber.Content}");
                    return;
                }
                
                if (!riddle.IsSolved)
                {
                    riddleIsSolved = false;
                    Console.WriteLine($"Du siehst im Raum: {chamber.Content}");
                    Console.WriteLine($"1: Gehe zu {target.GetType().Name} (ID: {target.ID})");
                    Console.WriteLine("2: Öffne Rucksack.");
                }
            }
            else
            {
                Console.WriteLine($"ERROR: conductChamberACTION(): Das Content {chamber.Content} in diesem Raum ist ungültig. Das darf nicht sein.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Der Raum ist leider LEER.");
            Console.WriteLine("Was möchtest du tun?");
            Console.WriteLine($"1: Gehe zu {target.GetType().Name} (ID: {target.ID})");
        }

        var leaveChamber = false;
        int choice = 0;
        if (chamberHasContent)
        {
            if (chamberHasFinding)
            {
                //Der Raum enthält einen Gegenstand.
                //Auswahl: 1: Rausgehen, 2: Gegenstand aufnehmen
                if (getValidUserInput(out choice))
                {
                    if (choice == 1)
                    {
                        //Den Raum verlassen
                        LeaveRoom(chamber);
                    }
                    else if (choice == 2) 
                    {
                        //Gegenstand aufnehmen
                        if(player != null)
                        {
                            player.Inventory.Add(chamber.Content);
                        }
                    }
                }
            }
            else
            {
                //Es ist ein Rätsel
                if (!riddleIsSolved)
                {
                    //Der Raum enthält ein Rätsel.
                    //Auswahl: 1: Rausgehen, 2: Inventar öffnen
                    if (getValidUserInput(out choice))
                    {
                        if (choice == 1)
                        {
                            //Den Raum verlassen
                            LeaveRoom(chamber);
                        }
                        else if (choice == 2)
                        {
                            //Element aus Inventar nehmen
                            //Auf Rätsel anwenden und aus dem Inventory löschen.
                            //Dann ist es fertig, beim nächsten Durchgang sollte das Rätsel nicht mehr da sein.

                            //Inventory öffnen
                            if (player != null)
                            {
                                List<string> bagpack = player.Inventory;

                                //player.Inventory.Add(chamber.Content);

                                for (int i = 0; i < bagpack.Count; i++)
                                {
                                    Console.WriteLine($"{i + 1}: {bagpack[i]}");
                                }
                                Console.WriteLine("Welchen Gegenstand willst du nehmen?");


                                if (bagpack.Count >= 1)
                                {
                                    if (getValidUserInput(out choice))
                                    {
                                        if(choice < bagpack.Count) { }

                                        var thing = bagpack[choice - 1];
                                        RiddlesRepository.ResultOfAction(chamber.Content, thing);
                                        bagpack.RemoveAt(choice - 1);
                                    }

                                }
                                else
                                {
                                    Console.WriteLine("Dein Rucksack ist leer.");
                                }

                            }
                        }
                    }
                } else
                {
                    //Das Rätsel ist gelöst, raus hier.
                    //Auswahl: 1: Rausgehen
                    if (getValidUserInput(out choice))
                    {
                        Console.WriteLine("Das Rätsel hier ist gelöst.");
                        Console.WriteLine("Was möchtest du tun?");
                        Console.WriteLine($"1: Gehe zu {target.GetType().Name} (ID: {target.ID})");
                        if (choice == 1)
                        {
                            //Den Raum verlassen
                            LeaveRoom(chamber);
                        }
                    }
                }
            }
        } else
        {
            //Der Raum ist leer.
            //Auswahl: 1: Rausgehen
            if (getValidUserInput(out choice))
            {
                if (choice == 1)
                {
                    //Den Raum verlassen
                    LeaveRoom(chamber);
                }
            }
        }

        Console.WriteLine("Was möchtest du tun?");
    }

    private static void LeaveRoom(Chamber chamber)
    {
        //Dient nur dazu den Raum zu verlassen. Setzt den Benutzer auf den ersten DockingPoint des Raums.
        var chosenPoint = chamber.DockingPoints[0];
        if (chosenPoint.ConnectedTo != null)
        {
            //es darf nichts null sein
            if (player != null && chosenPoint != null && chosenPoint.ConnectedTo != null)
            {
                //Der Spieler wird auf den neuen docking Point gesetzt
                player.CurrentDockingPoint = chosenPoint.ConnectedTo.PointB == chosenPoint
                    ? chosenPoint.ConnectedTo.PointA
                    : chosenPoint.ConnectedTo.PointB;
            }
        }
    }

    private static void conductMOVE(MazeElement currentElement)
    {



        Console.WriteLine($"Du befindest dich in einem {currentElement.GetType().Name} (ID: {currentElement.ID}). Gelöste Rätsel {RiddlesRepository.AmountOfSolvedRiddles()}/{RiddlesRepository.AmountOfUnsolvedRiddles()}");
        Console.WriteLine("Was möchtest du tun?");

        var options = currentElement.DockingPoints.Select((dp, index) =>
            new { Index = index, DockingPoint = dp }).ToList();

        for (int i = 0; i < options.Count; i++)
        {
            var dp = options[i].DockingPoint;
            var target = dp.ConnectedTo?.PointA == dp ? dp.ConnectedTo.PointB.Parent : dp.ConnectedTo?.PointA.Parent;
            //Console.WriteLine($"{i + 1}: {(target == null ? "Erkunden (unbekannt)" : $"Gehe zu {target.GetType().Name} (ID: {target.ID})")}");
            Console.WriteLine($"{i + 1}: {(target == null ? "Erkunden (unbekannt)" : $"Gehe zu {(target.ID != currentElement.ID ? target.GetType().Name : "Wand")} (ID: {target.ID})")}");
        }

        int choice = 0;
        var isInputValid = getValidUserInput(out choice);



        //Werte den Spielerinput aus
        if (isInputValid && choice > 0 && choice <= options.Count)
        {
            //Hol die gewählte Option und den gewählten Docking Point
            var chosenPoint = options[choice - 1].DockingPoint;

            if (chosenPoint.ConnectedTo == null)
            {
                handleUndefinedConnection(chosenPoint);
            }

            //es darf nichts null sein
            if (player != null && chosenPoint != null && chosenPoint.ConnectedTo != null)
            {
                //Der Spieler wird auf den neuen docking Point gesetzt
                player.CurrentDockingPoint = chosenPoint.ConnectedTo.PointB == chosenPoint
                    ? chosenPoint.ConnectedTo.PointA
                    : chosenPoint.ConnectedTo.PointB;
            }
        }
        else
        {
            Console.WriteLine("Ungültige Eingabe. Bitte versuche es erneut.");
        }
    }

    private static void handleUndefinedConnection(DockingPoint chosenPoint)
    {
        //Der docking Point ist noch nicht connected. 
        // Es muss ein neues MazeElement erzeugt wenn der Zufallsgenerator 0 bringt oder 
        // wenn es kein MazeElement hat das noch einen freien DockingPoint hat.
        if (random.Next(4 ) != 0 || !repository.MazeElements.Any(e => e.DockingPoints.Any(dp => dp.ConnectedTo == null)))
        {
            handleMakeupMazeElement(chosenPoint); //Es wird ein gänzlich neues Element erstellt und ans bestehende Labyrinth angehängt.
        }
        else
        {
            //handleMakeupMazeElement(chosenPoint); //Only FOR TESTING
            handleMazeElementRecycle(chosenPoint); //Es wird ein bestehender Korridor oder Kreuzung verbunden.
        }
    }

    private static void handleMazeElementRecycle(DockingPoint chosenPoint)
    {
        //Es wird das erste genommen das noch einen freien Andockpunkt hat.
        var existingElement = repository.MazeElements
            .First(e => e.DockingPoints.Any(dp => dp.ConnectedTo == null));

        var freeDockingPoint = existingElement.DockingPoints.First(dp => dp.ConnectedTo == null);
        var newConnector = MagicMaze.CreateConnector(chosenPoint, freeDockingPoint);
        repository.Connectors.Add(newConnector);

        Console.WriteLine($"Ein bestehender {existingElement.GetType().Name} wurde verbunden (ID: {existingElement.ID}).");
    }

    private static void handleMakeupMazeElement(DockingPoint chosenPoint)
    {
        //Erstell ein neues und hänge es dazu.
        var newElementType = random.Next(6) switch
        {
            0 => "Corridor",
            1 => "TieIntersection",
            //1 => "Corridor",  //Only FOR TESTING
            2 => "Corridor",
            3 => "CrossIntersection",
            //3 => "Corridor",  //Only FOR TESTING
            4 => "Corridor",
            5 => "Corridor",
            _ => "Corridor"
        };

        var newElement = MagicMaze.CreateMazeElement(newElementType);
        repository.MazeElements.Add(newElement);

        //Hier muss festgelegt werden welche zwei Punkte für andere Korridore zur Verfügung stehen und
        //welche eine Wand enthalten, also wieder auf den aktuellen Korridor verweisen und welche Räume bekommen.
        if (newElement is Corridor corridor)
        {
            if (corridor.DockingPoints.Count >= 4) // Sicherstellen, dass genug DockingPoints vorhanden sind
            {
                intializeCorridorConnection(1, corridor);
                intializeCorridorConnection(3, corridor);
            }
        }

        var newConnector = MagicMaze.CreateConnector(chosenPoint, newElement.DockingPoints[0]);
        repository.Connectors.Add(newConnector);

        Console.WriteLine($"Ein neuer {newElementType} wurde entdeckt (ID: {newElement.ID}).");
    }

    private static bool getValidUserInput(out int choice)
    {
        // Spielerinput holen
        Console.WriteLine("Gib eine Option ein (Nummer, oder 'q' zum Beenden):");
        var input = Console.ReadLine();
        choice = 0;
        if (string.IsNullOrEmpty(input))
        {
            //Bei einer Leereingabe wird der Input null.
            return false;
        }

        if (input.ToLower() == "q")
        {
            gameRunning = false; //stopp the game loop!           
            Console.WriteLine("Spiel beendet.");
            return false;
        }

        var isValid = int.TryParse(input, out choice);
        if (isValid && choice <= 0) {
            //0 ist natürlich ungültig.
            return false;
        }

        return isValid;
    }

    private static void initGame()
    {
        RiddlesRepository.initRiddlesRepository();
        //Erzeugt die ersten zwei Korridore und den Connector
        var corridor1 = (Corridor)MagicMaze.CreateMazeElement("Corridor");
        intializeCorridorConnection(1, corridor1);
        intializeCorridorConnection(3, corridor1);
        var corridor2 = (Corridor)MagicMaze.CreateMazeElement("Corridor");
        intializeCorridorConnection(1, corridor2);
        intializeCorridorConnection(3, corridor2);
        var connector = MagicMaze.CreateConnector(corridor1.DockingPoints[0], corridor2.DockingPoints[0]);

        repository.MazeElements.Add(corridor1);
        repository.MazeElements.Add(corridor2);
        repository.Connectors.Add(connector);

        // Spieler starten
        player = new Player(corridor1.DockingPoints[1]);
    }



    private static void intializeCorridorConnection(int index, Corridor corridor)
    {
        if (random.Next(2) == 0) // 50% Chance
        {
            //Wenn es keine Wand ist, soll es mit einem Raum verlinkt werden.
            var chamber = (Chamber)MagicMaze.CreateMazeElement("Chamber", RiddlesRepository.generateContent());
            var chamberConnector = MagicMaze.CreateConnector(corridor.DockingPoints[index], chamber.DockingPoints[0]);

            repository.MazeElements.Add(chamber);
            repository.Connectors.Add(chamberConnector);
        }
        else
        {
            //Wenn es eine Wand ist, soll es auf den aktuellen Korridor zurückführen.
            //Es muss die Fabric verwendet werden.
            var wallConnector = MagicMaze.CreateConnector(corridor.DockingPoints[index], corridor.DockingPoints[0]);
            repository.Connectors.Add(wallConnector);
        }
    }
}