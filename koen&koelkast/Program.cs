﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace koen_koelkast
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<State, State> states = new Dictionary<State, State>();
            string firstInput = Console.ReadLine();
            string[] splitInput = firstInput.Split(' ');
            int width = Int32.Parse(splitInput[0]);
            int height = Int32.Parse(splitInput[1]);
            string outputMode = splitInput[2];
            string[] field = new string[height];
            int i = 0;
            string fieldRead;
            State currentState = new State(0, 0, 0, 0);
            State end = new State(0, 0, 0, 0);
            while ((fieldRead = Console.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(fieldRead))
                {
                    break;
                }

                field[i] = fieldRead;
                if (fieldRead.Contains('+'))
                {
                    currentState.state += (uint)fieldRead.IndexOf('+') << 24;
                    currentState.state += (uint)i << 16;

                }

                if (fieldRead.Contains('?'))
                {
                    end.state += (uint)fieldRead.IndexOf('?') << 8;
                    end.state += (uint)i;
                }

                if (fieldRead.Contains('!'))
                {
                    currentState.state += (uint)fieldRead.IndexOf('!') << 8;
                    currentState.state += (uint)i;
                }

                i++;
            }
            State state = FindPath(ref states, field, end, currentState);
            int count = 0;
            if (state.CompareKoen(new State(0, 0, 0, 0)))
            {
                Console.WriteLine("No solution");
            }
            else
            {
                while (!state.CompareKoen(currentState))
                {
                    count++;
                    state = states[state];
                }
                if (outputMode == "P")
                {
                    Console.WriteLine(count);
                }
                else if (outputMode == "L")
                {
                    Console.WriteLine(count);
                    Console.WriteLine("not yet implemented");
                }
            }
        }

        static State FindPath(ref Dictionary<State, State> states, string[] field, State end, State begin)
        {
            Queue<State> stateQueue = new Queue<State>(field[1].Length * field.Length);
            List<State> newStates = new List<State>(4);
            states[begin] = new State(0, 0, 0, 0);
            stateQueue.Enqueue(begin);
            while (stateQueue.Count > 0)
            {
                State u = stateQueue.Dequeue();
                newStates.Add(North(u));
                newStates.Add(South(u));
                newStates.Add(West(u));
                newStates.Add(East(u));
                foreach (State s in newStates)
                {
                    if (s.koelkastX() >= field[1].Length || s.koelkastY() >= field.Length || s.koenX() >= field[1].Length || s.koenY() >= field.Length)
                    {
                        continue;
                    }

                    if (IsObstacle(field, s))
                    {
                        continue;
                    }

                    if (s.CompareFridge(end))
                    {
                        states.Add(s, u);
                        //Console.WriteLine(s.koenX() + " : " + s.koenY() + " ---- " + s.koelkastX() + " : " + s.koelkastY());
                        return s;
                    }

                    if (states.ContainsKey(s))
                    {
                        continue;
                    }
                    stateQueue.Enqueue(s);
                    states.Add(s, u);
                    Console.WriteLine(s.koenX() + " : " + s.koenY() + " ---- " + s.koelkastX() + " : " + s.koelkastY());
                }
                newStates.Clear();
            }

            return new State(0,0,0,0);
        }

        static bool IsObstacle(string[] field, State s)
        {
            try
            {
                string obstacle = field[s.koenY()][(int)s.koenX()].ToString();
                string pattern = @"^[A-Z]{1}$";
                if (Regex.IsMatch(obstacle, pattern))
                {
                    return true;
                }

                obstacle = field[s.koelkastY()][(int)s.koelkastX()].ToString();
                return Regex.IsMatch(obstacle, pattern);
            }
            catch { return false; }
        }

        static State North(State prev)
        {
            prev.state -= (1 << 16);
            if (prev.state >> 16 == (prev.state & 65535))
            {
                prev.state -= 1;
            }
            return prev;
        }

        static State South(State prev)
        {
            prev.state += 1 << 16;
            if (prev.state >> 16 == (prev.state & 65535))
            {
                prev.state += 1;
            }
            return prev;
        }

        static State West(State prev)
        {
            prev.state -= (1 << 24);
            if (prev.state >> 16 == (prev.state & 65535))
            {
                prev.state -= 1 << 8;
            }
            return prev;
        }

        static State East(State prev)
        {
            prev.state += 1 << 24;
            if (prev.state >> 16 == (prev.state & 65535))
            {
                prev.state += 1 << 8;
            }
            return prev;
        }
    }
}
    struct State
    {
        public State(uint koenX, uint koenY, uint koelkastX, uint koelkastY)
        {
            state = koenX << 24;
            state += koenY << 16;
            state += koelkastX << 8;
            state += koelkastY;
        }
        
        public uint state { get; set; }

        public bool CompareKoen(object obj)
        {
            if (obj is State other)
            {
                if (other.state >> 16 == state >> 16)
                    return true;
            }
            return false;
        }

        public bool CompareFridge(object other)
        {
            if (other is State s)
            {
                if (s.koelkastX() == koelkastX() && s.koelkastY() == koelkastY())
                {
                    return true;
                }
            }
            return false;
        }

        public uint koenX()
        {
            return state >> 24;
        }

        public uint koenY()
        {
            return (state >> 16) & 255;
        }

        public uint koelkastX()
        {
            return (state & 65535) >> 8;
        }

        public uint koelkastY()
        {
            return(state & 65535) & 255;
        }
    }