using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_5_manual_Task_1
{
    public class SymbolTable<TKey, TValue>
    {
        private class Entry
        {
            public TKey Key;
            public TValue Value;
            public Entry Next;

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Next = null;
            }
        }

        private readonly int _size;  // Size of the hash table
        private readonly Entry[] _table;  // Array of entries

        public SymbolTable(int size)
        {
            _size = size;
            _table = new Entry[size];
        }

        // Hash function to compute the index
        private int Hash(TKey key)
        {
            return Math.Abs(key.GetHashCode()) % _size;
        }

        // Insert a new key-value pair
        public void Insert(TKey key, TValue value)
        {
            int index = Hash(key);
            Entry newEntry = new Entry(key, value);

            if (_table[index] == null)
            {
                _table[index] = newEntry;
            }
            else
            {
                // Collision handling using chaining
                Entry current = _table[index];
                while (current != null)
                {
                    // Update value if the key already exists
                    if (current.Key.Equals(key))
                    {
                        current.Value = value;
                        return;
                    }
                    if (current.Next == null) break;
                    current = current.Next;
                }
                current.Next = newEntry;  // Add new entry at the end of the chain
            }
        }

        // Retrieve a value by key
        public TValue Retrieve(TKey key)
        {
            int index = Hash(key);
            Entry current = _table[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    return current.Value;  // Return the value if key is found
                }
                current = current.Next;  // Move to the next entry in the chain
            }
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        // Remove a key-value pair
        public void Remove(TKey key)
        {
            int index = Hash(key);
            Entry current = _table[index];
            Entry previous = null;

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    if (previous == null)
                    {
                        _table[index] = current.Next;  // Remove head of the chain
                    }
                    else
                    {
                        previous.Next = current.Next;  // Bypass the current entry
                    }
                    return;
                }
                previous = current;
                current = current.Next;
            }
            throw new KeyNotFoundException($"Key '{key}' not found.");
        }

        // Check if the symbol table contains a key
        public bool ContainsKey(TKey key)
        {
            int index = Hash(key);
            Entry current = _table[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    return true;  // Key found
                }
                current = current.Next;  // Move to the next entry in the chain
            }
            return false;  // Key not found
        }
    }

    // Example usage
    public class Program
    {
        public static void Main()
        {
            var symbolTable = new SymbolTable<string, int>(10);
            symbolTable.Insert("x", 10);
            symbolTable.Insert("y", 20);

            Console.WriteLine("Symbol table using hash function:");
            Console.WriteLine("Value for 'x' in Symbol Table : " + symbolTable.Retrieve("x")); // Output: 10
            Console.WriteLine("Value for 'y'  in Symbol Table: " + symbolTable.Retrieve("y")); // Output: 20

            symbolTable.Remove("x");

            Console.WriteLine("Symbol Table contains 'x': " + symbolTable.ContainsKey("x")); // Output: False
            Console.ReadKey();
        }
    }

}
