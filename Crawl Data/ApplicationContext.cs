using Crawl_Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Mynt.Core.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection.Metadata;
using System.Collections.Generic;
using System;
using Parameter = Crawl_Data.Models.Parameter;
using System.Text;

namespace Crawl_Data
{
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration _configuration;
        // public ApplicationContext(IConfiguration configuration)
        // {
        //     _configuration = configuration;
        // }

        // public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        // {
        // }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_configuration["MySqlConnetionString"]);
            }            

        }
        

        public string ExecuteStoreProcedure(string storeProcedure, List<Models.Parameter> arrParam)
        {
            try
            {
                
                using (var conn = new MySqlConnection(_configuration["MySqlConnetionString"]))
                using (var command = new MySqlCommand(storeProcedure, conn)
                { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    if (arrParam != null)
                        foreach (Models.Parameter param in arrParam)
                            command.Parameters.Add(new MySqlParameter(param.Varible, param.Value));
                    IDataReader reader = command.ExecuteReader();

                    var items = new List<Dictionary<string, object>>();
                    while (reader.Read())
                    {
                        var item = new Dictionary<string, object>(reader.FieldCount);
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            item[reader.GetName(i)] = reader.GetValue(i);
                        }

                        items.Add(item);
                    }

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);

                    reader.Close();
                    return json;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(arrParam);
                throw;
            }
        }

        public string select(string storeProcedure, List<Parameter> arrParam)
        {
            try
            {
                using (var conn = new MySqlConnection("server=192.168.1.220;Port=4306;database=examples;uid=root;password=P@ssw0rd;"))
                using (var command = new MySqlCommand(storeProcedure, conn)
                { CommandType = CommandType.StoredProcedure })
                {
                    conn.Open();
                    if (arrParam != null)
                        foreach (Parameter param in arrParam)
                            command.Parameters.Add(new MySqlParameter(param.Varible, param.Value));
                    IDataReader reader = command.ExecuteReader();
                    int size = reader.FieldCount;
                    List<String> arrFied = new List<string>(size);
                    StringBuilder strBuilder = new StringBuilder();
                    bool isRead = reader.Read();
                    for (int i = 0; i < size; i++)
                    {
                        arrFied.Add(reader.GetName(i));
                    }
                    strBuilder.Append("[");
                    while (isRead)
                    {
                        strBuilder.Append("{");
                        for (int i = 0; i < size; i++)
                        {
                            string name = arrFied[i];
                            //name = String.Concat(name.Substring(0, 1).ToLower(), name.Substring(1));
                            if (i != size - 1)
                            {
                                if (reader[name] is System.DateTime)
                                {
                                    string formated = String.Format("{0:M/d/yyyy hh:mm:ss tt}", reader[name]);
                                    strBuilder.Append("'").Append(name).Append("':'").Append(formated).Append("',");
                                }
                                else
                                {
                                    string val = reader[arrFied[i]].ToString();
                                    if (val.Contains("'"))
                                    {
                                        val = val.Replace("'", "\"");
                                    }
                                    strBuilder.Append("'").Append(name).Append("':'").Append(val).Append("',");
                                }
                            }
                            else
                            {
                                if (reader[name] is System.DateTime)
                                {
                                    string formated = String.Format("{0:M/d/yyyy hh:mm:ss tt}", reader[name]);
                                    strBuilder.Append("'").Append(name).Append("':'").Append(formated).Append("'}");
                                }
                                else
                                    strBuilder.Append("'").Append(name).Append("':'").Append(reader[arrFied[i]].ToString()).Append("'}");
                            }
                        }
                        if (reader.Read())
                        {
                            isRead = true;
                            strBuilder.Append(",");
                        }
                        else
                        {
                            isRead = false;
                        }
                    }
                    strBuilder.Append("]");
                    reader.Close();
                    return strBuilder.ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(arrParam);
                throw;
            }
        }

        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Candle> Candle { get; set; }
    }
}