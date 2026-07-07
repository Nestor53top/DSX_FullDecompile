namespace SQLitePCL;

public delegate int delegate_authorizer(object user_data, int action_code, utf8z param0, utf8z param1, utf8z dbName, utf8z inner_most_trigger_or_view);
