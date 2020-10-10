
using System;
using System.Collections.Generic;
using System.Reflection;

class Options
{
    public bool brief = false;
    public bool prependfile = false;
}

class TypePrinter
{
    Options m_opts;
    string m_filename;
    Assembly m_asm;

    public TypePrinter(Options opts, string filename)
    {
        m_opts = opts;
        m_filename = filename;
        m_asm = Assembly.ReflectionOnlyLoadFrom(m_filename);
    }

    private void PrependFilename()
    {
        if(m_opts.prependfile)
        {
            Console.Write("{0}:", m_filename);
        }
    }

    private void PWriteLine(string fmt, params object[] args)
    {
        PrependFilename();
        if(args.Length > 0)
        {
            Console.WriteLine(fmt, args);
        }
        else
        {
            Console.WriteLine(fmt);
        }
    }

    public void PrintTypes()
    {
        foreach(var type in m_asm.GetTypes())
        {
            if(m_opts.brief)
            {
                PWriteLine("{0}", type);
            }
            else
            {
                PWriteLine("class {0}", type.FullName);
                PWriteLine("{");
                foreach(var mth in type.GetMethods())
                {
                    PWriteLine("    {0};", mth);
                }
                PWriteLine("}");
            }
        }
    }
}

class ParseOptions
{
    public string[] origargs;
    public List<string> newargs;
    public Options opts;

    public ParseOptions(string[] args)
    {
        origargs = args;
        opts = new Options();
    }

    public bool Parse()
    {
        int i;
        string arg;
        newargs = new List<string>();
        for(i=0; i<origargs.Length; i++)
        {
            arg = origargs[i];
            if(arg[0] == '-')
            {
                if(arg.ToLower() == "-b")
                {
                    opts.brief = true;
                }
                else if(arg.ToLower() == "-f")
                {
                    opts.prependfile = true;
                }
                else
                {
                    Console.WriteLine("error: unknown/unhandled flag \"{0}\"\n", arg);
                    return false;
                }
            }
            else
            {
                newargs.Add(arg);
            }
        }
        return true;
    }
}


class Program
{
    public static int Main(string[] args)
    {
        int i;
        int len;
        string arg;
        var po = new ParseOptions(args);
        var nargs = po.Parse();
        if(po.Parse())
        {
            if((len = po.newargs.Count) == 0)
            {
                Console.WriteLine("usage: csnm <file> [<fileN...>]");
                return 1;
            }
            for(i=0; i<len; i++)
            {
                arg = po.newargs[i];
                var tp = new TypePrinter(po.opts, arg);
                tp.PrintTypes();
            }
        }
        else
        {
            Console.WriteLine("errors occured in command line options");
            return 1;
        }
        return 0;
    }
}

