/* 
Credits to Monky for... all of this
*/

using System;
using System.Collections.Generic;

namespace TMOD;

public class Command
{
    private static LogType _currentLogType; // "Success" by default
    public string Name { get; }
    public List<string> Aliases { get; } = new();
    public List<string> AutoParams { get; }
    public static char CmdPrefix = ConfigHandler.GetEntry<string>("CommandPrefix").Length == 1
        ? ConfigHandler.GetEntry<string>("CommandPrefix")[0]
        : '/';

    private readonly Action<string[], Command> _runCmdAction;
    private readonly int _minExpectedArgs;
    private bool _isPublic;
    public bool IsPublic
    {
        get => _isPublic;
        set
        {
            _isPublic = value;
        }
    }

    private static string _currentOutputMsg;
    public Command(string name, Action<string[], Command> cmdMethod, int minNumExpectedArgs, bool defaultPrivate,
        List<string> autoParameters = null)
    {
        Name = CmdPrefix + name;
        _runCmdAction = cmdMethod;
        _minExpectedArgs = minNumExpectedArgs;
        AutoParams = autoParameters;
        IsPublic = !defaultPrivate;
    }
    public void Execute(params string[] args)
    {
        if (args.Length < _minExpectedArgs)
        {
            _currentLogType = LogType.Error;
            _currentOutputMsg = "Invalid Parameters!";
            Helper.SendModOutput(_currentOutputMsg, _currentLogType, false);
                
            _currentLogType = LogType.Success;
            _currentOutputMsg = ""; 
            return;
        }

        try
        {
            _runCmdAction(args, this);
        }
        catch (Exception e)
        {
           ModLogger.Log("Exception occured when running command: " + e);

            _currentOutputMsg = "Something went wrong!";
            Helper.SendModOutput(_currentOutputMsg, LogType.Warning, false);
            _currentOutputMsg = "";
            throw;
        }

        if (string.IsNullOrEmpty(_currentOutputMsg)) // In case the command has no output
            return;
            
        if (_currentLogType == LogType.Warning)
        {
            Helper.SendModOutput(_currentOutputMsg, LogType.Warning, false);
            _currentLogType = LogType.Success;
            _currentOutputMsg = "";
            return;
        }

        Helper.SendModOutput(_currentOutputMsg, LogType.Success, IsPublic);
        _currentLogType = LogType.Success;
        _currentOutputMsg = "";
    }
    public enum LogType
    {
        Success,
        Warning,
        Error,
        Info

    }
}