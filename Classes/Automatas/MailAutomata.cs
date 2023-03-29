using System.Collections.Generic;
using System.Text;

namespace Crawler.Classes.Automata;

public class MailAutomata
{
    public enum State
    {
        Start,
        Username,
        Arroba,
        Domain,
        Dot,
        TLD,
        // Consideration, if the text ends with a dot, this must know if is part of the domain or if is the end of a paragraph
        Dot2,
        // Special state, used for send the buffer to the list of coincidences, if TLD and other value outside the alphabet then END
        End,
        // Special state, used for remove the buffer, if not TLD and value outside the alphabet
        Out
    }

    public State ActualState { get; set; }

    public List<string> Matches;

    private string Text { get; set; }

    private StringBuilder _buffer;

    public MailAutomata(string text)
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
                if (CheckIfIsFromAlphabet(Text[i]))
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
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Username;
                }
                else
                {
                    ActualState = State.Out;
                }
                break;
            case State.Username:
                if (value == '@')
                {
                    _buffer.Append(value);
                    ActualState = State.Arroba;
                }
                else if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                }
                else if (value == '.' || value == '-')
                {
                    _buffer.Append(value);
                    ActualState = State.Start;
                }
                else ActualState = State.Out;
                break;
            case State.Arroba:
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                    ActualState = State.Domain;
                }
                else
                {
                    ActualState = State.Out;
                }
                break;
            case State.Domain:
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                }
                else if (value == '.')
                {
                    _buffer.Append(value);
                    ActualState = State.Dot;
                }
                else ActualState = State.Out;
                break;
            case State.Dot:
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                    ActualState = State.TLD;
                }
                else ActualState = State.Out;
                break;
            case State.TLD:
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                }
                else if (value == '.')
                {
                    _buffer.Append(value);
                    ActualState = State.Dot2;
                }
                else {
                    ActualState = State.End;
                }
                break;
            case State.Dot2:
                if (CheckIfIsFromAlphabet(value))
                {
                    _buffer.Append(value);
                    ActualState = State.TLD;
                }
                else {
                    _buffer.Remove(_buffer.Length - 1, 1);
                    ActualState = State.End;
                }
                break;
        }
    }

    private bool CheckIfIsFromAlphabet(char value)
    {
        // If is a number
        if (value >= 48 && value <= 57) return true;

        // If is a lowercase
        if (value >= 97 && value <= 122) return true;

        // If is a uppercase
        if (value >= 65 && value <= 90) return true;

        return false;
    }

}