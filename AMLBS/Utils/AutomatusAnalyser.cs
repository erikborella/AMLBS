using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLBS.Utils
{
    internal static class AutomatusAnalyser
    {
        internal static T[] Analyse<T>(Queue<T> q, Predicate<T> predicate)
        {
            List<T> output = new();

            do
            {
                output.Add(q.Dequeue());

            } while (q.Count != 0 && predicate(q.Peek()));

            return output.ToArray();
        }
    }
}
