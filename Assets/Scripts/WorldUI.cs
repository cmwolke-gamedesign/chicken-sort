using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour {

    public float showTime = 2f;
    private float timer;

    private Text score;
    private Text details;
    
    private void Update()
    {
        this.timer += Time.deltaTime;
        this.GetComponent<RectTransform>().Translate(new Vector3(0, 0.005f));

        Color newColor = this.score.color;
        newColor.a = Mathf.Lerp(255, 0, timer / showTime);
        this.score.color = newColor;
        this.details.color = newColor;

        if (this.timer > showTime)
        {
            GameObject.Destroy(gameObject);
        }
    }
    

    public void SetText(Vector2 pos, Color color, string score, string[] details)
    {
        this.GetComponent<RectTransform>().SetPositionAndRotation(pos, Quaternion.identity);
        if (this.details == null)
            this.details = transform.Find("Details").GetComponent<Text>();
        if (this.score == null)
            this.score = transform.Find("Score").GetComponent<Text>();
        this.score.color = color;
        this.details.color = color;
        this.score.text = "+"+score;
        if (details.Length > 0)
        {
            this.details.text = details[0];
            for (int i = 1; i < details.Length; i++)
            {
                this.details.text += "\n" + details[i];
            }
        } else
        {
            this.details.text = "";
            this.score.GetComponent<RectTransform>().anchoredPosition.Set(3, 0);
        }
    }
}
