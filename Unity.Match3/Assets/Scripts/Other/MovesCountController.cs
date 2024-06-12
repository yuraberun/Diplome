using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovesCountController
{
    private static int MovesLeft { get; set; }
    public static bool CanMove { get; private set; }

    public static Action OnLose { get; set; }

    public static void SetTurns(int moves)
    {
        MovesLeft = moves;
        CanMove = true;
        UnityEngine.MonoBehaviour.FindAnyObjectByType<GUIHUD>().UpdateMovesCount(MovesLeft);
    }

    public static void OnTurnUse()
    {
        MovesLeft--;
        CanMove = MovesLeft > 0;
        GUIHUD.Instance?.UpdateMovesCount(MovesLeft);

        if (MovesLeft <= 0)
        {
            OnLose?.Invoke();
            GUIResults.Instance.ShowResultsAndRestartScene("Lose", Color.red, 1.5f);
        }
    }
}
