﻿// =========================== LICENSE ===============================
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
// ======================== EO LICENSE ===============================

using System;

using System.Collections.Generic;
using System.Text;

using org.rufwork.mooresDb.infrastructure.tableParts;
using org.rufwork.mooresDb.infrastructure.serializers;
using org.rufwork.mooresDb.infrastructure.contexts;
using System.IO;

namespace org.rufwork.mooresDb.infrastructure.commands
{
    public class DropTableCommand
    {
        private TableContext _table;
        private DatabaseContext _database;

        public DropTableCommand(DatabaseContext database)
        {
            _database = database;
        }

        public void executeStatement(string strSql)
        {
            // NOTE: Forced case insensitivity.
            strSql = strSql.ToLower().TrimEnd(';');

            string[] astrCmdTokens = Utils.stringToNonWhitespaceTokens2(strSql);

            bool bQuickTokenCheck = astrCmdTokens.Length >= 3 && "drop" == astrCmdTokens[0].ToLower() && "table" == astrCmdTokens[1].ToLower();
            if (!bQuickTokenCheck)
            {
                throw new Exception("Illegal drop command -- Syntax DROP TABLE TableName;");
            }
            else
            {
                string strTableName = astrCmdTokens[2];

                _table = _database.getTableByName(strTableName);
                if (null == _table)
                {
                    throw new Exception("Table not found in database: " + strTableName);
                }
                File.Delete(_table.strTableFileLoc);
                _database.removeExistingTable(_table);
            }
        }

        public static void Main(string[] args)
        {

            DatabaseContext database = new DatabaseContext(MainClass.cstrDbDir);

            string strTableName = DateTime.Now.Ticks.ToString();

            // TODO: I think I'm missing an AS, right?
            string strSql = @"create TABLE " + strTableName + @"
                (ID INTEGER (4) PRIMARY KEY,
                CITY CHAR (35),
                STATE CHAR (2),
                LAT_N REAL (5),
                LONG_W REAL (5));";

            Console.WriteLine("Executing:" + System.Environment.NewLine + strSql);

            CreateTableCommand createTableCommand = new CreateTableCommand(database);
            createTableCommand.executeStatement(strSql);

            //database = new DatabaseContext(MainClass.cstrDbDir);
            strSql = @"DROP TABLE " + strTableName + ";";

            DropTableCommand dropTableCommand = new DropTableCommand(database);
            dropTableCommand.executeStatement(strSql);

            Console.WriteLine("Return to quit.");
            Console.Read();
        }
    }
}