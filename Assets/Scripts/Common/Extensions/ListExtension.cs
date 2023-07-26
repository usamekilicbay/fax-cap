namespace Assets.Scripts.Common.Extensions
{
    using System.Collections.Generic;

    public static class ListExtension
    {
        /// <summary>
        /// Removes a specified number of elements from the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to remove elements from.</param>
        /// <param name="elementsToRemove">The number of elements to remove from the end of the list.</param>
        public static void RemoveLast<T>(this List<T> list, int elementsToRemove)
        {
            // Ensure elementsToRemove is a positive value and not exceeding the number of elements in the list
            if (elementsToRemove <= 0 || elementsToRemove > list.Count)
                return; // Nothing to remove or invalid number of elements to remove

            // Calculate the starting index of the elements to remove from the end of the list
            int startIndex = list.Count - elementsToRemove;

            // Remove the specified number of elements from the list starting from the calculated index
            list.RemoveRange(startIndex, elementsToRemove);
        }
    }
}
