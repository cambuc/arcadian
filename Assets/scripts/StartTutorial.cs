using System.Collections.Generic;
using UnityEngine;

public class StartTutorial : MonoBehaviour
{
    public float initialDelay;
    public float timeBetweenMessages;

    [System.Serializable]
    public struct Message
    {
        public GameObject indicator;
        public float timeOnScreen;
        public List<string> controls;
    }
    public List<Message> messages = new List<Message>();

    float newMessageTimer;
    float showingMessageTimer;

    private void Start()
    {
        foreach (Message m in messages)
        {
            m.indicator.SetActive(false);
        }
        newMessageTimer = initialDelay;
    }

    int i = 0;
    bool messageShown;
    void Update()
    {
        if(i >= messages.Count)
        {
            Destroy(this);
            return;
        }

        newMessageTimer -= Time.deltaTime;
        showingMessageTimer -= Time.deltaTime;

        if (newMessageTimer <= 0)
        {
            if(!messageShown)
            {
                messageShown = true;
                messages[i].indicator.SetActive(true);
                showingMessageTimer = messages[i].timeOnScreen;
            }

            foreach(string control in messages[i].controls)
            {
                if (Input.GetKey(control)) showingMessageTimer = 0;
            }

            if(showingMessageTimer <= 0)
            {
                messageShown = false;
                messages[i].indicator.SetActive(false);
                newMessageTimer = timeBetweenMessages;
                i++;
            }
        }
    }
}
