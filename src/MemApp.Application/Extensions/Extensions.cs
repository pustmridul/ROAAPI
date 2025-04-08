using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Extensions
{
    public interface IThrow
    {
    }
    public class Throw : IThrow
    {
        public static IThrow Exception { get; } = new Throw();


        private Throw()
        {
        }
    }
    public static class CustomExtensions
    {
        public static void IfNull<T>(T value, string propertyName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(propertyName);
            }
        }
        public static DataTable ConvertToDataTable<T>(T item)
        {
            DataTable dataTable = new DataTable();
            Type type = typeof(T);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType != null)
                {
                    dataTable.Columns.Add(property.Name, property.PropertyType);
                }
            }

                DataRow row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    if (property.PropertyType != null)
                    {
                        row[property.Name] = property.GetValue(item);
                    }              
                }

                dataTable.Rows.Add(row);


            return dataTable;
        }

        public static DataTable ConvertListToDataTable<T>(List<T> list)
        {
            DataTable dataTable = new DataTable();

            // Get the type of the object in the list
            Type type = typeof(T);

            // Get all the public properties of the object
            var properties = type.GetProperties();

            // Create the columns in the DataTable based on the object's properties
            try
            {
                foreach (var property in properties)
                {
                    dataTable.Columns.Add(property.Name, property.PropertyType);
                }
            }
            catch (Exception ex) {


                throw ex;
                    }

            // Populate the DataTable with data from the list
            foreach (var item in list)
            {
                DataRow row = dataTable.NewRow();

                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static void IfNull<T>( T value, string propertyName, string message)
        {
            if (value == null)
            {
                throw new ArgumentNullException(propertyName + " is NULL. " + message);
            }
        }

        public static void IfNotNull<T>(this IThrow validatR, T value, string message)
        {
            if (value != null)
            {
                throw new ArgumentException(message);
            }
        }

        public static void IfNullOrWhiteSpace(this IThrow validatR, string value, string propertyName)
        {
            IfNull(value, propertyName);
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Paramater " + propertyName + " cannot be empty.");
            }
        }

        public static void IfNotEqual<T>(this IThrow validatR, int valueOne, int valueTwo, string property)
        {
            if (valueOne != valueTwo)
            {
                throw new ArgumentException("Supplied " + property + " Values are not equal.");
            }
        }

        public static void IfFalse(this IThrow validatR, bool value, string message)
        {
            if (!value)
            {
                throw new ArgumentException(message);
            }
        }

        public static void IfTrue(this IThrow validatR, bool value, string message)
        {
            if (value)
            {
                throw new ArgumentException(message);
            }
        }

        public static void IfZero(this IThrow validatR, int value, string property)
        {
            if (value == 0)
            {
                throw new ArgumentException("This Property " + property + " Cannot be Zero");
            }
        }
    }
}
