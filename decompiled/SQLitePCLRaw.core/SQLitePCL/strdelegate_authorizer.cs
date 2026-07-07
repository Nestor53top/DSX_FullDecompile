namespace SQLitePCL;

public delegate int strdelegate_authorizer(object user_data, int action_code, string param0, string param1, string dbName, string inner_most_trigger_or_view);
