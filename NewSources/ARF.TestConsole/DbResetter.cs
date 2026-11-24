using ARF.Data.DbModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ARF.TestConsole
{
    public static class DbResetter
    {
        private static ArfDbEntities _db;
        private static string[] _acceptedComments = { "set", "use", "create ", "insert ", "delete ", "drop ", "alter ", "if " };

        public static ArfDbEntities DbContext
        {
            get
            {
                if (_db == null)
                {
                    _db = new ArfDbEntities();
                }

                return _db;
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static void ResetDatabase(string connStr = null, string fileName = "db_init.sql")
        {
            var db = DbContext;

            if (string.IsNullOrWhiteSpace(connStr))
            {
                connStr = db.Database.Connection.ConnectionString;
            }

            var rootPath = (string)null;
            for (var i = 1; i < 5; i++)
            {
                var append = "";
                for (var j = 0; j < i; j++)
                    append += "\\..";

                var dbScriptFile = AssemblyDirectory + append + "\\Database\\" + fileName;
                if (File.Exists(dbScriptFile))
                {
                    rootPath = AssemblyDirectory + append + "\\Database";
                    break;
                }
            }

            try
            {
                var connection = new SqlConnection(connStr.Replace("ecs_unit_testing", "master"));

                connection.Open();

                var filesToExecute = new string[] { fileName };
                foreach (var f in filesToExecute)
                {
                    var dbScriptFile = rootPath + "\\" + f;
                    if (File.Exists(dbScriptFile))
                    {
                        FileInfo fileInfo = new FileInfo(dbScriptFile);
                        var reader = fileInfo.OpenText();
                        string script = reader.ReadToEnd();
                        reader.Close();

                        var statements = SplitSql(script);
                        foreach (var statement in statements)
                        {
                            if (string.IsNullOrWhiteSpace(statement)) continue;
                            var command = connection.CreateCommand();
                            command.CommandText = statement;
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Failed to execute: " + command.CommandText + "\r\n" + ex.Message);
                            }
                        }
                    }
                }

                connection.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public static List<string> SplitSql(string theQuery)
        {
            var statements = new List<string>();

            var lines = theQuery.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var waitingForGoStatement = false;
            var waitingForEndStatement = false;
            var waitingForNewCommand = false;
            var waitingForCommentEnd = false;
            var statement = "";
            foreach (var line in lines)
            {
                if (statement.Contains("CREATE TRIGGER [DELETE_BACKGROUNDJOBS] ON [dbo].[BackgroundJobs] INSTEAD OF DELETE AS BEGIN"))
                {
                }
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (line.Trim().ToUpper() == "GO")
                {
                    statements.Add(statement);
                    statement = "";
                    waitingForNewCommand = true;
                    waitingForCommentEnd = false;
                    waitingForGoStatement = false;
                    waitingForEndStatement = false;

                    continue;
                }

                if (statement.ToLower().StartsWith("create trigger") && statement.ToLower().Contains("begin") && !statement.ToLower().Contains("end"))
                {
                    waitingForEndStatement = true;
                }

                if (waitingForGoStatement)
                {
                    if (!waitingForEndStatement)
                    {
                        if (line.ToLower().Trim().StartsWith("go"))
                        {
                            statements.Add(statement);
                            statement = "";
                            waitingForGoStatement = false;
                            waitingForNewCommand = true;
                        }
                        else
                        {
                            statement += "\r\n" + line;
                        }
                    }
                    else
                    {
                        statement += "\r\n" + line;

                        if (line.ToLower().Trim().StartsWith("end"))
                        {
                            waitingForEndStatement = false;
                        }
                    }

                    continue;
                }

                var line2 = line.Trim();
                if (line2.StartsWith("--")) continue;

                if (waitingForCommentEnd)
                {
                    if (line2.EndsWith("*/"))
                    {
                        waitingForCommentEnd = false;
                        continue;
                    }
                }
                else
                {
                    if (line2.StartsWith("/*"))
                    {
                        if (!line2.EndsWith("*/"))
                            waitingForCommentEnd = true;
                        continue;
                    }

                    if (waitingForEndStatement)
                    {
                        if (line2.ToLower().EndsWith("end") || line2.ToLower().EndsWith("end;"))
                        {
                            if (statement.EndsWith(" "))
                                statement += line2;
                            else
                                statement += " " + line2;

                            waitingForEndStatement = false;
                        }
                        else
                        {
                            if (statement.EndsWith(" "))
                                statement += line2;
                            else
                                statement += " " + line2;
                        }
                    }
                    else
                    {
                        if (line2.ToLower().StartsWith("if") && !(line2.ToLower().EndsWith("end") || line2.ToLower().EndsWith("end;")))
                        {
                            if (statement.EndsWith(" "))
                                statement += line2;
                            else
                                statement += " " + line2;

                            waitingForEndStatement = true;

                            continue;
                        }
                        else
                        {
                            if (waitingForNewCommand)
                            {
                                var foundNewCommand = false;
                                foreach (var cmd in _acceptedComments)
                                {
                                    if (line2.ToLower().StartsWith(cmd))
                                    {
                                        foundNewCommand = true;
                                        break;
                                    }
                                }

                                if (foundNewCommand)
                                {
                                    if (!string.IsNullOrWhiteSpace(statement))
                                        statements.Add(statement.Trim());
                                    statement = line2;
                                    if (line2.EndsWith(";"))
                                    {
                                        statements.Add(statement.Trim());
                                        statement = "";
                                        waitingForNewCommand = true;
                                    }
                                }
                                else
                                {
                                    if (statement.EndsWith(" "))
                                        statement += line2;
                                    else
                                        statement += " " + line2;
                                }
                            }
                            else
                            {
                                var foundNewCommand = false;
                                foreach (var cmd in _acceptedComments)
                                {
                                    if (line2.ToLower().StartsWith(cmd))
                                    {
                                        foundNewCommand = true;
                                        break;
                                    }
                                }

                                if (foundNewCommand)
                                {
                                    if (!string.IsNullOrWhiteSpace(statement))
                                    {
                                        statements.Add(statement.Trim());
                                    }

                                    statement = line2;

                                    if (!line2.EndsWith(";"))
                                    {
                                        waitingForNewCommand = true;
                                    }
                                    else
                                    {
                                        statements.Add(statement.Trim());
                                        statement = "";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(statement))
                statements.Add(statement);

            return statements;
        }
    }
}