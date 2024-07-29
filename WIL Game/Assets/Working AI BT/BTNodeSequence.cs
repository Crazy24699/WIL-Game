using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class BTNodeSequence : BTNodeBase
//{
//    public NodeStateOptions CurrentNodeStatus;
//    protected List<BTNodeBase> AllNodes;
    
//    public void SetSequenceValues(List<BTNodeBase> NodeList)
//    {
//        AllNodes = NodeList;
//    }

//    //I do know what this does. I know how to use it as well. Shocker i know. 
//    public override NodeStateOptions RunLogicAndState()
//    {
//        bool AnyNodesActive = false;
//        foreach (var Node in AllNodes)
//        {
//            switch (Node.RunLogicAndState())
//            {
//                case NodeStateOptions.Running:
//                    AnyNodesActive = true;
//                    break;
//                case NodeStateOptions.Failed:
//                    CurrentNodeState=NodeStateOptions.Failed;
//                    break;
//                case NodeStateOptions.Passed:
//                    break;

//            }
//        }

//        CurrentNodeState = AnyNodesActive ? NodeStateOptions.Running : NodeStateOptions.Passed;
//        return CurrentNodeState;
//    }

//}
