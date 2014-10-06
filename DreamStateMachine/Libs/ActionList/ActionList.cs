using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    class ActionList
    {
        private Actor owner;
        private List<Action> actions;
        private Action curAction;

        public ActionList(Actor o)
        {
            actions = new List<Action>();
            owner = o;
        }

        public void endAll()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                curAction = actions.ElementAt(i);
                curAction.onEnd();
            }
        }

        public void update(float dt){
            for (int i = 0; i < actions.Count; i++)
            {
                curAction = actions.ElementAt(i);
                if (!curAction.isStarted)
                {
                    curAction.onStart();
                    curAction.isStarted = true;
                }

                curAction.update(dt);

                if (curAction.duration <= curAction.elapsed && curAction.duration != -1)
                    curAction.onEnd();

                if (curAction.isBlocking)
                    i = actions.Count;

                //Console.WriteLine("Elapsed:" + curAction.elapsed);
                //Console.WriteLine("Duration:" + curAction.duration);

                    
            }

        }

        public void pushFront(Action action){
            //Console.WriteLine("push is getting called");
            //Console.WriteLine(actions.Count);
            actions.Insert(0, action);
            //Console.WriteLine(actions.Count);
        }

        public void pushBack(Action action)
        {
            actions.Add(action);
        }

        public Action remove(Action action)
        {
            Action toRemove = action;
            //actions.Remove(action);
            actions.RemoveAll(x => x.GetType() == action.GetType());
            return toRemove;
        }

        public bool has(Action action)
        {
            //Console.WriteLine(action.GetType());
            //Console.WriteLine("Num actions in has:" + actions.Count);
            if (actions.Count > 0)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    Action curAction = actions.ElementAt(i);
                    //Console.WriteLine(curAction.GetType());
                    if (curAction.GetType().Equals(action.GetType()))
                    {
                        //Console.WriteLine("this worked");
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public Action begin()
        {
            return actions.ElementAtOrDefault(0);
        }

        public Action end()
        {
            return actions.ElementAt(actions.Count - 1);
        }

        public bool isEmpty()
        {
            return actions.Count == 0;
        }

        public float timeLeft()
        {
            Action curAction = this.begin();
            float totalTime = 0;
            while (!curAction.isBlocking)
                totalTime += (curAction.duration - curAction.elapsed);

            return totalTime;
        }

        public int getSize()
        {
            return actions.Count();
        }
        

    }
}
