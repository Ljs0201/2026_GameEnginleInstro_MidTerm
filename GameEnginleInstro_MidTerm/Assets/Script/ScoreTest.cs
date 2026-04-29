using TMPro;
using UnityEngine;

public class ScoreTagset : MonoBehaviour
{

    public TextMeshProUGUI stage1;
    public TextMeshProUGUI stage2;
    void Start()
    {
        stage1.text = "StaGE 1 : " + HighScore.Load(1).ToString();
        stage2.text = "StaGE 2 : " + HighScore.Load(2).ToString();
    }
}
