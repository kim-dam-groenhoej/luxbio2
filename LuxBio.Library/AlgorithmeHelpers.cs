// Author: Kim Dam Grønhøj, Gitte Nielsen
using LuxBio.Library.Models.ExtraPropperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxBio.Library
{
    public static class AlgorithmeHelpers
    {
        public static List<Chair> FindCombinedObjects(this List<LuxBio.Library.Models.ExtraPropperties.Chair> source, int seatsCount)
        {
            var chairs = new List<Chair>();
            var groupsChairs = new List<List<Chair>>();

            // grouping possible choices and filter only available chairs
            GroupingChairs(source, groupsChairs);

            var choosedGroupChairs = new List<Chair>();
            var found = false;
            int i = 0;
            // search after best group choice
            while (!found && i <= (groupsChairs.Count() - 1))
            {
                var gc = groupsChairs[i];
                // Is there enough chairs available in group?
                if (seatsCount <= gc.Count())
                {
                    found = true;
                    // Take the first "x" chairs in group
                    chairs = gc.Take(seatsCount).ToList();
                }
                i++;
            }

            // Return wanted chairs for a row
            return chairs;
        }

        private static void GroupingChairs(List<Chair> source, List<List<Chair>> groupsChairs)
        {
            // add instance of List<Chair>()
            var currentGroup = new List<Chair>();
            groupsChairs.Add(currentGroup);

            var newSource = source.Where(c => c.Available == Library.Models.ExtraPropperties.ChairAvailableType.Available).ToList();
            foreach (var chair in newSource)
            {
                int rightChairIndex = newSource.FindIndex(c => c.ID == chair.ID) + 1;
                // Add chair to group
                currentGroup.Add(chair);

                if (rightChairIndex < newSource.Count())
                {
                    // get right chair
                    var rightChair = newSource[rightChairIndex];
                    // Is "right chair's ordernumber - left chair's ordernumber" higher than 1
                    if ((rightChair.OrderNumber - chair.OrderNumber) > 1)
                    {
                        currentGroup = new List<Chair>();
                        // add new instance of List<Chair>()
                        groupsChairs.Add(currentGroup);
                    }
                }
            }
        }

        public static List<Chair> FindCombinedObjectsByFirstChoice(this List<LuxBio.Library.Models.ExtraPropperties.Chair> source, Chair firstChair, int seatsCount)
        {
            var chairs = new List<Chair>();
            if (firstChair.Available == ChairAvailableType.Available)
            {
                if (seatsCount == 1)
                {
                    chairs.Add(firstChair);
                    return chairs;
                }

                var groupsChairs = new List<List<Chair>>();

                // grouping possible choices and filter only available chairs
                GroupingChairs(source, groupsChairs);

                var groupChairs = groupsChairs.FirstOrDefault(gc => gc.FirstOrDefault(c => c.ID == firstChair.ID) != null).ToList();

                // Is all available chairs there?
                if (seatsCount <= groupChairs.Count())
                {
                    chairs.Add(firstChair);

                    var foundChairs = false;
                    // current index for selected chair
                    int currentIndex = groupChairs.FindIndex(gc => gc.ID == firstChair.ID);
                    int leftIndex = currentIndex - 1;
                    int rightIndex = currentIndex + 1;

                    // Check that index is not lower than the collection
                    if (leftIndex < 0) leftIndex = 0;
                    if (leftIndex == currentIndex) leftIndex = -1;

                    // Check that index is not higher than the collection
                    if (rightIndex > groupChairs.Count() - 1) rightIndex = groupChairs.Count() - 1;
                    if (rightIndex == currentIndex) rightIndex = groupChairs.Count() + 1;

                    while (!foundChairs)
                    {
                        // find all left chairs
                        if (leftIndex >= 0 && !foundChairs)
                        {
                            var leftChair = groupChairs[leftIndex];
                            chairs.Add(leftChair);
                            leftIndex--;
                        }

                        if (chairs.Count() >= seatsCount)
                        {
                            foundChairs = true;
                        }

                        // find all right chairs
                        if (rightIndex <= groupChairs.Count() - 1 && !foundChairs)
                        {
                            var rightChair = groupChairs[rightIndex];
                            chairs.Add(rightChair);
                            rightIndex++;
                        }

                        if (chairs.Count() >= seatsCount)
                        {
                            foundChairs = true;
                        }
                    }
                }
            }

            return chairs;
        }

        public static List<T> OrderByMiddle<T>(this List<T> source)
        {
            var copied = new List<T>(source); // used to preserve orginal List of rows being manipulated
            var newCollection = new List<T>();

            // get middle index number by divide the number of rows
            int middleIndex = Convert.ToInt32(Math.Round(Convert.ToDecimal(source.Count() / 2)));
            // find the object by the index and adding to the new list
            newCollection.Add(source[middleIndex]);
            // remove the object from the copied list and send the list to the method itself again later.
            copied.Remove(source[middleIndex]);
            // Is there more objects left, if not: stop calling itself
            if (copied.Count() > 0)
            {
                newCollection.AddRange(OrderByMiddle<T>(copied));
            }
            // return the new list
            return newCollection;
        }
    }
}
