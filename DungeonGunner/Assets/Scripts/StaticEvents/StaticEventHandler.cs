using System;
using UnityEngine;

public static class StaticEventHandler { 
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room) {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room});
    }

    public static event Action<RoomEnemiesDefeatedEventArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedEventArgs() { room = room });
    }

    public static event Action<PointsScoredArgs> OnPointsScored;

    public static void CallPointsScoredEvent(int points)
    {
        OnPointsScored?.Invoke(new PointsScoredArgs() { points = points });
    }

    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score = score });
    }
}

public class RoomChangedEventArgs : EventArgs {  
    public Room room;
}

public class RoomEnemiesDefeatedEventArgs : EventArgs
{
    public Room room;
}

public class PointsScoredArgs : EventArgs
{
    public int points;
}

public class ScoreChangedArgs : EventArgs
{
    public long score;
}
