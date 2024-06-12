using System;
using System.Collections;
using System.Collections.Generic;
using Common.Interfaces;
using Match3.App;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGoalController : MonoBehaviour
{
    private static int Goal { get; set; }
    public static int ItemID { get; private set; }
    public static bool CanMove { get; private set; }

    public static Action OnWin { get; set; }

    public static void SetLevelGoal(int moves, int itemID)
    {
        CanMove = true;
        Goal = moves;
        ItemID = itemID;
        UnityEngine.MonoBehaviour.FindAnyObjectByType<GUIHUD>().UpdateGoalsIcon(ItemID);
        UnityEngine.MonoBehaviour.FindAnyObjectByType<GUIHUD>().UpdateGoalsCount(Goal);
    }

    public static void OnMoveEnd(SolvedData<IUnityGridSlot> data)
    {
        int count = 0;

        foreach (var sequence in data.SolvedSequences)
        {
            foreach (var item in sequence.SolvedGridSlots)
            {
                if (item.ItemId == ItemID)
                {
                    count++;
                }
            }
        }

        Goal = Math.Max(0, Goal - count);
        UnityEngine.MonoBehaviour.FindAnyObjectByType<GUIHUD>().UpdateGoalsCount(Goal);
    }

    public static bool CheckWin()
    {
        if (Goal <= 0)
        {
            CanMove = false;
            GUIResults.Instance.ShowResultsAndRestartScene("Win", Color.green, 2f);
            OnWin?.Invoke();
            return true;
        }

        return false;
    }
}
