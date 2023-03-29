using System.Collections.Generic;
using System.Text;

namespace Crawler.Classes.Automata;

class UrlAutomata
{
    public enum State
    {
        Start,
        // The protocol must consist only of letters
        Protocol,
        Colon,
        Slash1,
        Slash2,
        Hostname,
        Dot,
        // If is a subdomain then we can go back to Dot and re
        TLD,
        // Colon2 and Port can be omited if the next value is the SlashToPath
        ColonForPort,
        // This must be only numbers
        Port,
        SlashToPath,
        Path,
        // If the thing ends with a dot, must be considerated incomplete or like the end of some like a paragraph
        EOP,
        // After the path, the query and section must be considerated? 
        // For now, all next will be ignored
        End,
        // Special state, used for remove the buffer, if not TLD and value outside the alphabet
        Out
    }

    public State ActualState { get; set; }

    public List<string> Matches;

    private string Text { get; set; }

    private StringBuilder _buffer;

    public UrlAutomata(string text)
    {
        Matches = new();
        Text = text;
        _buffer = new();
        ActualState = State.Start;
        Match();
    }

    private void Match()
    {
        for (int i = 0; i < Text.Length; i++)
        {
            Eval(Text[i]);
            if (ActualState == State.End)
            {
                Matches.Add(_buffer.ToString());
                _buffer.Clear();
                ActualState = State.Start;
            }
            else if (ActualState == State.Out)
            {
                _buffer.Clear();
                ActualState = State.Start;
                if(IsLetter(Text[i]))
                {
                    Eval(Text[i]);
                }
            }

        }

    }

    private void Eval(char value)
    {
        switch (ActualState)
        {
            case State.Start:
                if (IsLetter(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Protocol;
                }
                break;
            case State.Protocol:
                if (IsLetter(value)) _buffer.Append(value);
                else if (value == ':')
                {
                    _buffer.Append(value);
                    ActualState = State.Colon;
                }
                else ActualState = State.Out;
                break;
            case State.Colon:
                if (value == '/')
                {
                    _buffer.Append(value);
                    ActualState = State.Slash1;
                }
                else ActualState = State.Out;
                break;
            case State.Slash1:
                if (value == '/')
                {
                    _buffer.Append(value);
                    ActualState = State.Slash2;
                }
                else ActualState = State.Out;
                break;
            case State.Slash2:
                if (IsLetter(value) || IsNumber(value) || IsScore(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Hostname;
                }
                else ActualState = State.Out;
                break;
            case State.Hostname:
                if (IsLetter(value) || IsNumber(value) || IsScore(value)) _buffer.Append(value);
                else if (value == '.')
                {
                    _buffer.Append(value);
                    ActualState = State.Dot;
                }
                else
                    ActualState = State.Out;
                break;
            case State.Dot:
                if (IsLetter(value))
                { 
                    _buffer.Append(value);
                    ActualState = State.TLD;
                }
                else ActualState = State.Out;
                break;
            case State.TLD:
                if (IsLetter(value)) _buffer.Append(value);
                else if(value == '.')
                {
                    _buffer.Append(value);
                    ActualState = State.Dot;
                }
                else if(value == '/')
                {
                    _buffer.Append(value);
                    ActualState = State.SlashToPath;
                }
                else if(value == ':')
                {
                    _buffer.Append(value);
                    ActualState = State.ColonForPort;
                }
                else ActualState = State.End;
                break;
            case State.ColonForPort:
                if(IsNumber(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Port;
                }
                else ActualState = State.Out;
                break;
            case State.Port:
                if(IsNumber(value)) _buffer.Append(value);
                else if(value == '/') {
                    _buffer.Append(value);
                    ActualState = State.SlashToPath;
                }
                else ActualState = State.End;
                break;
            case State.SlashToPath:
                if(!IsSpace(value) && !IsBad(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Path;
                }
                else ActualState = State.End;
                break;
            case State.Path:
                if(!IsSpace(value) && !IsBad(value))
                {
                    _buffer.Append(value);
                }
                else ActualState = State.End;
                break;
        }
    }

    private bool IsNumber(char value)
    {
        if (value >= '0' && value <= '9') return true;
        return false;
    }

    private bool IsLetter(char value)
    {
        if (value >= 'a' && value <= 'z') return true;
        if (value >= 'A' && value <= 'Z') return true;
        return false;
    }

    private bool IsScore(char value) { return (value == '-' || value == '_'); }

    private bool IsSpace(char value) { return (value == '\r' || value == '\n' || value == ' '); }

    private bool IsBad(char value) { return (value == ' ' || value == '\'' || value == '"' || value == '`' || value == '\\' || value == '<' || value== '>' || value == '[' || value == ']'); }
}
