using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour
{
    public enum GameMode { TimeTrial, Zen, ThirtySwipes }
    public static GameMode SelectedGameMode { get; set; }
}
