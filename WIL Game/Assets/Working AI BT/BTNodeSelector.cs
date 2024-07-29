using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class BTNodeSelector : BTNodeBase
//{

//    protected List<BTNodeBase> AllNodes = new List<BTNodeBase>();

//    public BTNodeSelector(List<BTNodeBase> NodeList)
//    {
//        AllNodes = NodeList;

//    }

//    public override NodeStateOptions RunLogicAndState()
//    {
//        foreach (var Node in AllNodes)
//        {
//            switch (Node.RunLogicAndState())
//            {
//                case NodeStateOptions.Running:
//                    CurrentNodeState = NodeStateOptions.Running;
//                    break;
//                case NodeStateOptions.Failed:
//                    CurrentNodeState = NodeStateOptions.Failed;
//                    break;
//                case NodeStateOptions.Passed:
//                    CurrentNodeState = NodeStateOptions.Passed;
//                    break;
//                default:
//                    break;
//            }
//        }

//        CurrentNodeState = NodeStateOptions.Failed;
//        return CurrentNodeState;
//    }
//}
