using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod BM = TextArchitect.BuildMethod.Instant;

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
            architect.buildMethod = TextArchitect.BuildMethod.Fade;
            architect.Speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            if (BM != architect.buildMethod) 
            {
                architect.buildMethod = BM;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S)) 
            {
                architect.Stop();
            }


            string LongLine = "ThisIsAVeryLongLineWithALotOfLetters";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.IsBuilding)
                {
                    if (!architect.HurryUp)
                        architect.HurryUp = true;
                    else architect.ForceComplete();
                }
                else architect.Build(Lines[Random.Range(0, Lines.Length)]);
            }

            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(Lines[Random.Range(0, Lines.Length)]);
            }
                
        }
    }
}
