using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Text;
using System.Xml;
using System.IO;
using System.Data.Common;

namespace вывод_данных_из_бд
{
    class Program
    {
        static Dimension[] dimensions;

        static void readElementsInfoFromDB(SQLiteConnection connection, List<String> infoElements, String name, int idOfElement)
        {
            switch (name)
            {
                case "Customer":
                    {
                        String sqlQuery = ("SELECT Name From " + name + " where " + name.Insert(name.Length, "_id=") + idOfElement.ToString() + ";");
                        SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                        SQLiteDataReader reading = command.ExecuteReader();
                        foreach (DbDataRecord record in reading)
                            infoElements.Add(record.GetValue(0).ToString());
                        break;
                    }
                case "Shop":
                    {
                        String sqlQuery = ("SELECT Name From " + name + " where " + name.Insert(name.Length, "_id=") + idOfElement.ToString() + ";");
                        SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                        SQLiteDataReader reading = command.ExecuteReader();
                        foreach (DbDataRecord record in reading)
                            infoElements.Add(record.GetValue(0).ToString());
                        break;
                    }
                case "Apple":
                    {
                        String sqlQuery = ("SELECT Kind_of_apple From " + name + " where " + name.Insert(name.Length, "_id=") + idOfElement.ToString() + ";");
                        SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                        SQLiteDataReader reading = command.ExecuteReader();
                        foreach (DbDataRecord record in reading)
                            infoElements.Add(record.GetValue(0).ToString());
                        break;
                    }
                case "Date":
                    {
                        String sqlQuery = ("SELECT Day, Month, Year From " + name + " where " + name.Insert(name.Length, "_id=") + idOfElement.ToString() + ";");
                        SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                        SQLiteDataReader reading = command.ExecuteReader();
                        foreach (DbDataRecord record in reading)
                            infoElements.Add(record.GetValue(0) + "." + record.GetValue(1) + "." + record.GetValue(2));
                        break;
                    }
            }
        }

        static int getDimensionsIdByRealName(String realName)
        {
            for (int i = 0; i < dimensions.Length; i++)
                if (dimensions[i].RealName == realName)
                    return i;
            return -1;
        }

        static String getFieldsUserNameByRealName(String realName)
        {
            String tableName = realName.Split('.')[0];
            String fieldName = realName.Split('.')[1];
            for (int i = 0; i < dimensions.Length; i++)
                if (dimensions[i].RealName == tableName)
                    for (int j = 0; j < dimensions[i].Fields.Count; j++)
                        if (dimensions[i].Fields.ContainsKey(fieldName) == true)
                            return dimensions[i].Fields[fieldName].Name;
            return null;
        }

        static void Main(string[] args)
        {
            Console.SetWindowSize(140, Console.WindowHeight);    

            string nameOfColumn = null, nameOfRow = null, fixName = null;
            int[] idsOfColumnElements = null;
            int[] idsOfRowElements = null;
            int idOfFixElement = 0;
            List<String> outElements = new List<String>();
            List<String> infoColumnElements = new List<String>();
            List<String> infoRowElements = new List<String>();
            List<String> infoFixElement = new List<String>();



            XmlTextReader reader = new XmlTextReader(@"C:\Users\Гена\Desktop\visual c# and c++\c#\read xml\read xml\bin\Release\report.xml");
            XmlDocument doc = new XmlDocument();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    String[] temp;
                    switch (reader.Name)
                    {
                        case "DimensionNameOfColumn":
                            reader.Read();
                            nameOfColumn = reader.Value;
                            break;
                        case "IdsOfSelectedColumnElements":
                            reader.Read();
                            temp = reader.Value.Split(' ');
                            idsOfColumnElements = new int[temp.Length];
                            for (int i = 0; i < temp.Length; i++)
                                idsOfColumnElements[i] = Convert.ToInt32(temp[i]);
                            break;
                        case "DimensionNameOfRow":
                            reader.Read();
                            nameOfRow = reader.Value;
                            break;
                        case "IdsOfSelectedRowElements":
                            reader.Read();
                            temp = reader.Value.Split(' ');
                            idsOfRowElements = new int[temp.Length];
                            for (int i = 0; i < temp.Length; i++)
                                idsOfRowElements[i] = Convert.ToInt32(temp[i]);
                            break;
                        case "FixedName":
                            reader.Read();
                            fixName = reader.Value;
                            break;
                        case "FixedNameID":
                            reader.Read();
                            idOfFixElement = Convert.ToInt32(reader.Value);
                            break;
                        case "OutField":
                            reader.Read();
                            outElements.Add(reader.Value);
                            break;
                    }//end of switch
                }//end of if
            }//end of while
            reader.Close();

            doc.Load(@"C:\Users\Гена\Desktop\довнар описание xml (2).xml");

            XmlNodeList xmlDimensions = doc.SelectSingleNode("FullReport").LastChild.ChildNodes;
            dimensions = new Dimension[xmlDimensions.Count];
            for (int i = 0; i < xmlDimensions.Count; i++)
            {
                dimensions[i] = new Dimension();
                XmlNode dimInfo = xmlDimensions[i].SelectSingleNode("DimensionInformation");
                dimensions[i].Id = Convert.ToInt32(dimInfo.FirstChild.InnerText);
                dimensions[i].UserName = dimInfo.LastChild.InnerText;
                XmlNodeList xmlAttributes = xmlDimensions[i].SelectSingleNode("Attributes").ChildNodes;

                for (int j = 0; j < xmlAttributes.Count; j++)
                {
                    int id = Convert.ToInt32(xmlAttributes[j].SelectSingleNode("ID").InnerText);
                    String name = xmlAttributes[j].SelectSingleNode("Name").InnerText;
                    dimensions[i].Attributes.Add(new Attribute(id, name));
                }

                XmlNode tableInfo = xmlDimensions[i].SelectSingleNode("TableInformation");
                dimensions[i].RealName = tableInfo.SelectSingleNode("NameInDataBase").InnerText;
                dimensions[i].PrimaryKey = tableInfo.SelectSingleNode("PrimaryKeyInfo").SelectSingleNode("Name").InnerText;

                XmlNodeList xmlFields = xmlDimensions[i].SelectSingleNode("Fields").ChildNodes;
                for (int j = 0; j < xmlFields.Count; j++)
                {
                    String name = xmlFields[j].SelectSingleNode("Name").InnerText;
                    int attrId = Convert.ToInt32(xmlFields[j].SelectSingleNode("ReferenceOnAttribute").InnerText);
                    Attribute refOnAttr = null;
                    foreach (Attribute attr in dimensions[i].Attributes)
                        if (attr.Id == attrId)
                            refOnAttr = attr;
                    dimensions[i].Fields.Add(name, refOnAttr);
                }
            }//end of for

            FactTable factTable = new FactTable();
            XmlDocument factDoc = new XmlDocument();

            factDoc.Load(@"C:\Users\Гена\Desktop\довнар описание xml факты.xml");
            XmlNodeList xmlFacts = factDoc.SelectSingleNode("FullReport").SelectSingleNode("DataTable").ChildNodes;

            factTable.Name = xmlFacts[0].InnerText;
            factTable.PrimaryKey = xmlFacts[1].InnerText;

            XmlNodeList xmlFactFields = factDoc.SelectSingleNode("FullReport").SelectSingleNode("DataTable").SelectSingleNode("Fields").ChildNodes;
            for (int j = 0; j < xmlFactFields.Count; j++)
            {
                String nameOfField = xmlFactFields[j].SelectSingleNode("Name").InnerText;
                int tableId = Convert.ToInt32(xmlFactFields[j].SelectSingleNode("References").SelectSingleNode("DimensionTable").InnerText);
                String nameOfFieldInDimTable = xmlFactFields[j].SelectSingleNode("References").SelectSingleNode("NameOfField").InnerText;
                factTable.Fields.Add(nameOfField, new Tuple<int, string>(tableId, nameOfFieldInDimTable));
            }
            //////////////////////////////////////////////////////////////////////////////////

            FileInfo file = new FileInfo("output.txt");
            StreamWriter fOut = new StreamWriter(file.Create());
            const string databaseName = @"C:\Users\Гена\Desktop\visual c# and c++\c#\подсоединение sqlite\" +
                @"подсоединение sqlite\Apples and shops v2 - копия.db";

            SQLiteConnection connection = new SQLiteConnection(string.Format("Data Source={0};", databaseName));
            connection.Open();

            for (int i = 0; i < idsOfColumnElements.Length; i++)
                readElementsInfoFromDB(connection, infoColumnElements, nameOfColumn, idsOfColumnElements[i]);

            for (int i = 0; i < idsOfRowElements.Length; i++)
                readElementsInfoFromDB(connection, infoRowElements, nameOfRow, idsOfRowElements[i]);

            readElementsInfoFromDB(connection, infoFixElement, fixName, idOfFixElement);

            String[,,] pivotInfo = new String[idsOfRowElements.Length, idsOfColumnElements.Length, outElements.Count];

            for (int i = 0; i < idsOfRowElements.Length; i++)
            {
                for (int j = 0; j < idsOfColumnElements.Length; j++)
                {
                    String sqlQuery = "SELECT ";                                            //начинаем формировать запрос к базе данных
                    for (int k = 0; k < outElements.Count; k++)
                    {
                        sqlQuery = sqlQuery.Insert(sqlQuery.Length, outElements[k]);
                        if (k != outElements.Count - 1)
                            sqlQuery += ", ";
                    }

                    sqlQuery += (" FROM Customer INNER JOIN Sale ON (Sale.Customer_id=Customer.Customer_id) INNER JOIN Apple ON " +
                    "(Sale.Apple_id=Apple.Apple_id) INNER JOIN Shop ON (Sale.Shop_id=Shop.Shop_id) INNER JOIN Date " +
                    "ON (Sale.Date_id= Date.Date_id) WHERE ");
                    sqlQuery += fixName + "." + fixName.Insert(fixName.Length, "_id=") + idOfFixElement.ToString() + " AND ";
                    sqlQuery += nameOfColumn + "." + nameOfColumn.Insert(nameOfColumn.Length, "_id=") + idsOfColumnElements[j].ToString() + " AND ";
                    sqlQuery += nameOfRow + "." + nameOfRow.Insert(nameOfRow.Length, "_id=") + idsOfRowElements[i].ToString();
                    SQLiteCommand command = new SQLiteCommand(sqlQuery, connection);
                    SQLiteDataReader reading = command.ExecuteReader();
                    foreach (DbDataRecord record in reading)
                        for (int l = 0; l < outElements.Count; l++)
                            pivotInfo[i, j, l] = record.GetValue(l).ToString();
                }
            }
            connection.Close();

            String userColumnName = dimensions[getDimensionsIdByRealName(nameOfColumn)].UserName;
            String userRowName = dimensions[getDimensionsIdByRealName(nameOfRow)].UserName;
            String userFixName = dimensions[getDimensionsIdByRealName(fixName)].UserName;

            Console.WriteLine(userFixName + " " + infoFixElement[0]);
            Console.Write("\t\t\t\t");
            for (int i = 0; i < infoColumnElements.Count; i++)
                Console.Write(userColumnName + " " + infoColumnElements[i] + "\t\t");

            Console.WriteLine();

            for (int i = 0; i < infoRowElements.Count; i++)
            {
                Console.Write(userRowName + " " + infoRowElements[i]+ "\t\t");
                for (int j = 0; j < outElements.Count; j++)
                {
                    if (j != 0)
                        Console.Write("\t\t\t\t     ");
                    for (int k = 0; k < idsOfColumnElements.Length; k++)
                    {
                        Console.Write(getFieldsUserNameByRealName(outElements[j]) + "=" + pivotInfo[i,k,j]+"\t\t");
                    }
                    Console.WriteLine();
                }

            }
            //////////////////////////////////////////////////////////////////

            fOut.WriteLine(userFixName + " " + infoFixElement[0]);
            fOut.Write("\t\t\t\t");
            for (int i = 0; i < infoColumnElements.Count; i++)
                fOut.Write(userColumnName + " " + infoColumnElements[i] + "\t\t");

            fOut.WriteLine();

            for (int i = 0; i < infoRowElements.Count; i++)
            {
                fOut.Write(userRowName + " " + infoRowElements[i] + "\t\t");
                for (int j = 0; j < outElements.Count; j++)
                {
                    if (j != 0)
                        fOut.Write("\t\t\t\t     ");
                    for (int k = 0; k < idsOfColumnElements.Length; k++)
                    {
                        fOut.Write(getFieldsUserNameByRealName(outElements[j]) + "=" + pivotInfo[i, k, j] + "\t\t");
                    }
                    fOut.WriteLine();
                }
            }

            fOut.Close();


        }
    }
}
