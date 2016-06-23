using System;
using System.Collections.Generic;
using SQLitePCL;
using MyHub.Models.LocalDB;

namespace MyHub.Services
{
    /// <summary>
    /// 本地数据库访问方法【没有进行完全测试】
    /// </summary>
    public static class LocalDataAccessMethods
    {
        #region SQL语句

        private static readonly string DB_NAME = "MyHubDB.db";
        private static readonly string TABLE_NAME_SNSTYPE = "SnsTypeTable";
        private static readonly string TABLE_NAME_ACCOUNT = "AccountTable";
        private static readonly string TABLE_NAME_SETTING = "UserSettingTable";
        private static readonly string TABLE_NAME_ACTION_TYPE = "ActionTypeTable";
        private static readonly string TABLE_NAME_ACTION_HISTORY = "ActionHistoryTable";

        private static readonly string SQL_CREATE_TABLE_SNSTYPE = "create table if not exists " 
            + TABLE_NAME_SNSTYPE 
            + " (sns_id integer primary key asc," 
            + " sns_name text not null);";
        private static readonly string SQL_CREATE_TABLE_ACCOUNT = "create table if not exists " 
            + TABLE_NAME_ACCOUNT 
            + " (account_id integer primary key asc," 
            + " sns_id integer not null," 
            + " access_token text not null," 
            + " refresh_token text," 
            + " expires_in text not null,"
            + " user_id text,"
            + " user_name text,"
            + " user_logourl text,"
            + " is_available integer not null);";
        private static readonly string SQL_CREATE_TABLE_SETTING = "create table if not exists " 
            + TABLE_NAME_SETTING 
            + " (setting_id integer primary key asc," 
            + " setting_name text not null," 
            + " setting_value text);";
        private static readonly string SQL_CREATE_TABLE_ACTION_TYPE = "create table if not exists " 
            + TABLE_NAME_ACTION_TYPE 
            + " (type_id integer primary key asc,"
            + " type_name text not null);";
        private static readonly string SQL_CREATE_TABLE_ACTION_HISTORY = "create table if not exists " 
            + TABLE_NAME_ACTION_HISTORY 
            + " (action_id integer primary key asc,"
            + " sns_id integer,"
            + " type_id integer,"
            + " content text not null);";

        private static readonly string SQL_INSERT_SNSTYPE = "insert into " + TABLE_NAME_SNSTYPE + " values(?,?);";
        private static readonly string SQL_INSERT_ACCOUNT = "insert into " + TABLE_NAME_ACCOUNT + " values(?,?,?,?,?,?,?,?,?);";
        private static readonly string SQL_INSERT_SETTING = "insert into " + TABLE_NAME_SETTING + " values(?,?,?);";
        private static readonly string SQL_INSERT_ACTION_TYPE = "insert into " + TABLE_NAME_ACTION_TYPE + " values(?,?);";
        private static readonly string SQL_INSERT_ACTION_HISTORY = "insert into " + TABLE_NAME_ACTION_HISTORY + " values(?,?,?,?);";

        private static readonly string SQL_DELETE_SNSTYPE = "delete from " + TABLE_NAME_SNSTYPE + " where sns_id = ?;";
        private static readonly string SQL_DELETE_ACCOUNT = "delete from " + TABLE_NAME_ACCOUNT + " where account_id = ?;";
        private static readonly string SQL_DELETE_SETTING = "delete from " + TABLE_NAME_SETTING + " where setting_id = ?;";
        private static readonly string SQL_DELETE_ACTION_TYPE = "delete from " + TABLE_NAME_ACTION_TYPE + " where type_id = ?;";
        private static readonly string SQL_DELETE_ACTION_HISTORY = "delete from " + TABLE_NAME_ACTION_HISTORY + " where action_id = ?;";

        private static readonly string SQL_UPDATE_SNSTYPE = "update " 
            + TABLE_NAME_SNSTYPE 
            + " set sns_name = ?"
            + " where sns_id = ?;";
        private static readonly string SQL_UPDATE_ACCOUNT = "update " 
            + TABLE_NAME_ACCOUNT 
            + " set account_id = ?, access_token = ?, refresh_token = ?, expires_in = ?, user_id = ?, user_name = ?, user_logourl = ?, is_available = ?"
            + " where sns_id = ?;";
        private static readonly string SQL_UPDATE_SETTING = "update " 
            + TABLE_NAME_SETTING 
            + " set setting_name = ?, setting_value = ?"
            + " where setting_id = ?;";
        private static readonly string SQL_UPDATE_ACTION_TYPE = "update " 
            + TABLE_NAME_ACTION_TYPE 
            + " set type_name = ?"
            + " where type_id = ?;";
        private static readonly string SQL_UPDATE_ACTION_HISTORY = "update " 
            + TABLE_NAME_ACTION_HISTORY 
            + " set sns_id = ?, type_id = ?, content = ?"
            + " where action_id = ?;";

        // 在这个应用的业务领域内，根据一个条件进行查询足够了，不需要联合查询
        private static string SQL_QUERY_SNSTYPE { get { return "select * from " + TABLE_NAME_SNSTYPE + " where " + _query_key + " = ?;"; } }
        private static string SQL_QUERY_ACCOUNT { get { return "select * from " + TABLE_NAME_ACCOUNT + " where " + _query_key + " = ?;"; } }
        private static string SQL_QUERY_SETTING { get { return "select * from " + TABLE_NAME_SETTING + " where " + _query_key + " = ?;"; } }
        private static string SQL_QUERY_ACTION_TYPE { get { return "select * from " + TABLE_NAME_ACTION_TYPE + " where " + _query_key + " = ?;"; } }
        private static string SQL_QUERY_ACTION_HISTORY { get { return "select * from " + TABLE_NAME_ACTION_HISTORY + " where " + _query_key + " = ?;"; } }

        #endregion SQL语句

        private static SQLiteConnection _connection = null;
        private static ISQLiteStatement statement = null;
        private static List<string> _insert_parameters = new List<string>();// 实际参数不可包含主键参数
        private static List<string> _update_parameters = new List<string>();// 主键参数在最后
        private static string _query_key = "";

        #region 初始化数据库

        public static bool InitLocalDB()
        {
            try
            {
                _connection = new SQLiteConnection(DB_NAME);

                statement = _connection.Prepare(SQL_CREATE_TABLE_SNSTYPE);
                statement.Step();

                statement = _connection.Prepare(SQL_CREATE_TABLE_ACCOUNT);
                statement.Step();

                statement = _connection.Prepare(SQL_CREATE_TABLE_SETTING);
                statement.Step();

                statement = _connection.Prepare(SQL_CREATE_TABLE_ACTION_TYPE);
                statement.Step();

                statement = _connection.Prepare(SQL_CREATE_TABLE_ACTION_HISTORY);
                statement.Step();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static void CloseDB()
        {
            if(_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        #endregion 初始化数据库


        #region 插入数据库表项

        /// <summary>
        /// 插入方法的基方法，被其他插入方法调用
        /// </summary>
        /// <param name="sql">插入的sql语句</param>
        /// <param name="parameters">需要插入表中的参数，注意由于主键自动递增，所以参数数目应为属性数量-1</param>
        /// <returns></returns>
        private static bool InsertBase(string sql, List<string> parameters)
        {
            if (_connection == null)
                InitLocalDB();

            try
            {
                statement = _connection.Prepare(sql);

                for(int i = 1; i <= parameters.Count; ++i)
                {
                    statement.Bind(i + 1, parameters[i - 1]);
                }
                if (statement.Step() != SQLiteResult.DONE)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool Insert_SnsType(string sns_name)
        {
            _insert_parameters.Clear();
            _insert_parameters.Add(sns_name);

            return InsertBase(SQL_INSERT_SNSTYPE, _insert_parameters);
        }

        public static bool Insert_Account(string sns_id, string access_token, string refresh_token, string expires_in, string user_id, string user_name, string user_logourl, string is_available)
        {
            _insert_parameters.Clear();
            _insert_parameters.Add(sns_id);
            _insert_parameters.Add(access_token);
            _insert_parameters.Add(refresh_token);
            _insert_parameters.Add(expires_in);
            _insert_parameters.Add(user_id);
            _insert_parameters.Add(user_name);
            _insert_parameters.Add(user_logourl);
            _insert_parameters.Add(is_available);
            
            return InsertBase(SQL_INSERT_ACCOUNT, _insert_parameters);
        }

        public static bool Insert_Setting(string setting_name, string setting_value)
        {
            _insert_parameters.Clear();
            _insert_parameters.Add(setting_name);
            _insert_parameters.Add(setting_value);

            return InsertBase(SQL_INSERT_SETTING, _insert_parameters);
        }

        public static bool Insert_ActionType(string type_name)
        {
            _insert_parameters.Clear();
            _insert_parameters.Add(type_name);

            return InsertBase(SQL_INSERT_ACTION_TYPE, _insert_parameters);
        }

        public static bool Insert_ActionHistory(string sns_id, string type_id, string content)
        {
            _insert_parameters.Clear();
            _insert_parameters.Add(sns_id);
            _insert_parameters.Add(type_id);
            _insert_parameters.Add(content);

            return InsertBase(SQL_INSERT_ACTION_HISTORY, _insert_parameters);
        }

        #endregion 插入数据库表项


        #region 删除数据库表项

        /// <summary>
        /// 删除数据库表项的基方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool DeleteBase(string sql, string id)
        {
            if (_connection == null)
                InitLocalDB();

            try
            {
                statement = _connection.Prepare(sql);

                statement.Bind(1, id);
                if (statement.Step() != SQLiteResult.DONE)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool Delete_SnsType(string sns_id)
        {
            return DeleteBase(SQL_DELETE_SNSTYPE, sns_id);
        }

        public static bool Delete_Account(string account_id)
        {
            return DeleteBase(SQL_DELETE_ACCOUNT, account_id);
        }

        public static bool Delete_Setting(string setting_id)
        {
            return DeleteBase(SQL_DELETE_SETTING, setting_id);
        }

        public static bool Delete_ActionType(string type_id)
        {
            return DeleteBase(SQL_DELETE_ACTION_TYPE, type_id);
        }

        public static bool Delete_ActionHistory(string action_id)
        {
            return DeleteBase(SQL_DELETE_ACTION_HISTORY, action_id);
        }

        #endregion 删除数据库表项


        #region 更新数据库表项

        /// <summary>
        /// 更新数据库表项的基方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool UpdateBase(string sql, List<string> parameters)
        {
            if (_connection == null)
                InitLocalDB();

            try
            {
                statement = _connection.Prepare(sql);

                for(int i = 1; i <= parameters.Count; ++i)
                {
                    statement.Bind(i, parameters[i - 1]);
                }
                if (statement.Step() != SQLiteResult.DONE)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool Update_SnsType(SnsTypeTableEntity entity)
        {
            _update_parameters.Clear();
            _update_parameters.Add(entity.sns_name);
            _update_parameters.Add(entity.sns_id.ToString());

            return UpdateBase(SQL_UPDATE_SNSTYPE, _update_parameters);
        }

        public static bool Update_Account(AccountTableEntity entity)
        {
            _update_parameters.Clear();
            _update_parameters.Add(entity.account_id.ToString());
            _update_parameters.Add(entity.access_token);
            _update_parameters.Add(entity.refresh_token);
            _update_parameters.Add(entity.expires_in);
            _update_parameters.Add(entity.user_id);
            _update_parameters.Add(entity.user_name);
            _update_parameters.Add(entity.user_logourl);
            _update_parameters.Add(entity.is_available.ToString());
            _update_parameters.Add(entity.sns_id.ToString());

            return UpdateBase(SQL_UPDATE_ACCOUNT, _update_parameters);
        }

        public static bool Update_Setting(UserSettingTableEntity entity)
        {
            _update_parameters.Clear();
            _update_parameters.Add(entity.setting_name);
            _update_parameters.Add(entity.setting_value);
            _update_parameters.Add(entity.setting_id.ToString());

            return UpdateBase(SQL_UPDATE_SETTING, _update_parameters);
        }

        public static bool Update_ActionType(ActionTypeTableEntity entity)
        {
            _update_parameters.Clear();
            _update_parameters.Add(entity.type_name);
            _update_parameters.Add(entity.type_id.ToString());

            return UpdateBase(SQL_UPDATE_ACTION_TYPE, _update_parameters);
        }

        public static bool Update_ActionHistory(ActionHistoryTableEntity entity)
        {
            _update_parameters.Clear();
            _update_parameters.Add(entity.sns_id.ToString());
            _update_parameters.Add(entity.type_id.ToString());
            _update_parameters.Add(entity.content);
            _update_parameters.Add(entity.action_id.ToString());

            return UpdateBase(SQL_UPDATE_ACTION_HISTORY, _update_parameters);
        }

        #endregion 更新数据库表项


        #region 查找数据库表项

        /// <summary>
        /// 查找数据库表项的基方法
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="query_value">指定的查询字段的值</param>
        /// <returns>返回ISQLiteStatement的对象，在后续方法中继续解析</returns>
        private static ISQLiteStatement QueryBase(string sql, string query_value)
        {
            if (_connection == null)
                InitLocalDB();

            try
            {
                statement = _connection.Prepare(sql);

                statement.Bind(1, query_value);
                if (statement.Step() != SQLiteResult.ROW)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            return statement;
        }

        /// <summary>
        /// 【存在问题：只能查询到单个结果】
        /// </summary>
        /// <param name="query_key">sns_id, sns_name之一</param>
        /// <param name="query_value"></param>
        /// <returns></returns>
        public static SnsTypeTableEntity Query_SnsType(string query_key, string query_value)
        {
            SnsTypeTableEntity entity = new SnsTypeTableEntity();

            _query_key = query_key;
            statement = QueryBase(SQL_QUERY_SNSTYPE, query_value);
            if (statement == null)
                return null;

            entity.sns_id = (long)statement[0];
            entity.sns_name = statement[1] as string;

            return entity;
        }

        /// <summary>
        /// 【存在问题：只能查询到单个结果】
        /// </summary>
        /// <param name="query_key">account_id, sns_id, access_token, refresh_token, expires_in, user_id, user_name, user_logourl, is_available之一</param>
        /// <param name="query_value"></param>
        /// <returns></returns>
        public static AccountTableEntity Query_Account(string query_key, string query_value)
        {
            AccountTableEntity entity = new AccountTableEntity();

            _query_key = query_key;
            statement = QueryBase(SQL_QUERY_ACCOUNT, query_value);
            if (statement == null)
                return null;

            entity.account_id = (long)statement[0];
            entity.sns_id = (long)statement[1];
            entity.access_token = statement[2] as string;
            entity.refresh_token = statement[3] as string;
            entity.expires_in = statement[4] as string;
            entity.user_id = statement[5] as string;
            entity.user_name = statement[6] as string;
            entity.user_logourl = statement[7] as string;
            entity.is_available = (long)statement[8];

            return entity;
        }

        /// <summary>
        /// 【存在问题：只能查询到单个结果】
        /// </summary>
        /// <param name="query_key">setting_id, setting_name, setting_value之一</param>
        /// <param name="query_value"></param>
        /// <returns></returns>
        public static UserSettingTableEntity Query_Setting(string query_key, string query_value)
        {
            UserSettingTableEntity entity = new UserSettingTableEntity();

            _query_key = query_key;
            statement = QueryBase(SQL_QUERY_SETTING, query_value);
            if (statement == null)
                return null;

            entity.setting_id = (long)statement[0];
            entity.setting_name = statement[1] as string;
            entity.setting_value = statement[2] as string;

            return entity;
        }

        /// <summary>
        /// 【存在问题：只能查询到单个结果】
        /// </summary>
        /// <param name="query_key">type_id, type_name之一</param>
        /// <param name="query_value"></param>
        /// <returns></returns>
        public static ActionTypeTableEntity Query_ActionType(string query_key, string query_value)
        {
            ActionTypeTableEntity entity = new ActionTypeTableEntity();

            _query_key = query_key;
            statement = QueryBase(SQL_QUERY_ACTION_TYPE, query_value);
            if (statement == null)
                return null;
            
            entity.type_id = (long)statement[0];
            entity.type_name = statement[1] as string;

            return entity;
        }

        /// <summary>
        /// 【存在问题：只能查询到单个结果】
        /// </summary>
        /// <param name="query_key">action_id, sns_id, type_id, content之一</param>
        /// <param name="query_value"></param>
        /// <returns></returns>
        public static ActionHistoryTableEntity Query_ActionHistory(string query_key, string query_value)
        {
            ActionHistoryTableEntity entity = new ActionHistoryTableEntity();

            _query_key = query_key;
            statement = QueryBase(SQL_QUERY_ACCOUNT, query_value);
            if (statement == null)
                return null;

            entity.action_id = (long)statement[0];
            entity.sns_id = (long)statement[1];
            entity.type_id = (long)statement[2];
            entity.content = statement[3] as string;

            

            return entity;
        }

        #endregion 查找数据库表项
    }
}
