using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        string[] Lines = new string[5]
        {
            "This is a random line",
            "Something comes over here",
            "This is the third sentence",
            "This is the fourth sentence",
            "The last oneeeee"
        };
        
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogeContainer.DialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.Typewriter;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                architect.Build(Lines[Random.Range(0, Lines.Length)]);
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(Lines[Random.Range(0, Lines.Length)]);
            }
                
        }
    }
}
