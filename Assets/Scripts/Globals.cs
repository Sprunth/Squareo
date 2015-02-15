using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour
{
    public enum GameMode { TimeTrial, Zen }
    public static GameMode SelectedGameMode { get; set; }
}
