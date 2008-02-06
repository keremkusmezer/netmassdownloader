/*------------------------------------------------------------------------------
 * Debugging Microsoft .NET 2.0 Applications
 * Copyright © 1997-2006 John Robbins -- All rights reserved. 
 -----------------------------------------------------------------------------*/
using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace NetMassDownloader
{
    /// <summary>
    /// A command line argument parsing class.
    /// </summary>
    /// <remarks>
    /// This class is based on the WordCount version from the Framework SDK
    /// samples.  Any errors are mine.
    /// <para>
    /// There are two arrays of flags you'll pass to the constructors.  The
    /// flagSymbols are supposed to be standalone switches that toggle an option
    /// on.  The dataSymbols are for switches that take data values.  For 
    /// example, if your application needs a switch, -c, to set the count, 
    /// you'd put "c" in the dataSymbols.  This code will allow both "-c100" and 
    /// the usual "-c" "100" both to be passed on the command line.  Note that
    /// you can pass null/Nothing for dataSymbols if you don't need them.
    /// </para>
    /// </remarks>
    internal abstract class ArgParser
    {
        // For example: "/", "-"
        private String [] switchChars;
        // Switch character(s) that are simple flags
        private String [] flagSymbols;
        // Switch characters(s) that take parameters.  For example: -f <file>.
        // This can be null if not needed.
        private String [] dataSymbols;
        // Are switches case-sensitive?
        private Boolean caseSensitiveSwitches;


        /// <summary>
        /// The status values for various internal methods.
        /// </summary>
        protected enum SwitchStatus
        {
            /// <summary>
            /// Success.
            /// </summary>
            NoError ,
            /// <summary>
            /// There was a problem.
            /// </summary>
            Error ,
            /// <summary>
            /// Show the usage help.
            /// </summary>
            ShowUsage
        } ;

        ///// <summary>
        ///// Constructs the class with nothing but flag switches and defaults to 
        ///// "/" and "-" as valid switch characters.
        ///// </summary>
        ///// <param name="flagSymbols">
        ///// The array of simple flags to toggle options on or off. 
        ///// </param>
        //protected ArgParser ( String [] flagSymbols )
        //    : this ( flagSymbols ,
        //             null ,
        //             false ,
        //             new string [] { "/" , "-" } )
        //{
        //}

        ///// <summary>
        ///// Constructs the class with nothing but flag switches and defaults to 
        ///// "/" and "-" as valid switch characters.
        ///// </summary>
        ///// <param name="flagSymbols">
        ///// The array of simple flags to toggle options on or off. 
        ///// </param>
        ///// <param name="caseSensitiveSwitches">
        ///// True if case sensitive switches are supposed to be used.
        ///// </param>
        //protected ArgParser ( String [] flagSymbols ,
        //                      Boolean caseSensitiveSwitches )
        //    : this ( flagSymbols ,
        //             null ,
        //             caseSensitiveSwitches ,
        //             new string [] { "/" , "-" } )
        //{
        //}

        /// <summary>
        /// Constructs the class to use case-insensitive switches and 
        /// defaults to "/" and "-" as valid switch characters.
        /// </summary>
        /// <param name="flagSymbols">
        /// The array of simple flags to toggle options on or off. 
        /// </param>
        /// <param name="dataSymbols">
        /// The array of options that need data either in the next parameter or
        /// after the switch itself.  This value can be null/Nothing.
        /// </param>
        protected ArgParser ( String [] flagSymbols , String [] dataSymbols )
            : this ( flagSymbols ,
                     dataSymbols ,
                     false ,
                     new string [] { "/" , "-" } )
        {
        }

        ///// <summary>
        ///// Constructs the class and defaults to "/" and "-" as the only 
        ///// valid switch characters
        ///// </summary>
        ///// <param name="flagSymbols">
        ///// The array of simple flags to toggle options on or off. 
        ///// </param>
        ///// <param name="dataSymbols">
        ///// The array of options that need data either in the next parameter or
        ///// after the switch itself.  This value can be null/Nothing.
        ///// </param>
        ///// <param name="caseSensitiveSwitches">
        ///// True if case sensitive switches are supposed to be used.
        ///// </param>
        //protected ArgParser ( String [] flagSymbols ,
        //                      String [] dataSymbols ,
        //                      Boolean caseSensitiveSwitches )
        //    : this ( flagSymbols ,
        //             dataSymbols ,
        //             caseSensitiveSwitches ,
        //             new string [] { "/" , "-" } )
        //{
        //}


        /// <summary>
        /// Constructor where the caller sets all options to the class.
        /// </summary>
        /// <param name="flagSymbols">
        /// The array of simple flags to toggle options on or off. 
        /// </param>
        /// <param name="dataSymbols">
        /// The array of options that need data either in the next parameter or
        /// after the switch itself.  This value can be null/Nothing.
        /// </param>
        /// <param name="caseSensitiveSwitches">
        /// True if case sensitive switches are supposed to be used.
        /// </param>
        /// <param name="switchChars">
        /// The array of switch characters to use.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="flagSymbols"/> or 
        /// <paramref name="switchChars"/> are invalid.
        /// </exception>
        protected ArgParser ( String [] flagSymbols ,
                              String [] dataSymbols ,
                              Boolean caseSensitiveSwitches ,
                              String [] switchChars )
        {
            Debug.Assert ( null != flagSymbols , "null != flagSymbols" );
            // Avoid assertion side effects in debug builds.
#if DEBUG
            if ( null != flagSymbols )
            {
                Debug.Assert ( flagSymbols.Length > 0 ,
                               "flagSymbols.Length > 0" );
            }
#endif
            if ( ( null == flagSymbols ) || ( 0 == flagSymbols.Length ) )
            {
                throw new ArgumentException ( Constants.ArrayMustBeValid ,
                                              "flagSymbols" );
            }
            Debug.Assert ( null != switchChars , "null != switchChars" );
            // Avoid assertion side effects in debug builds.
#if DEBUG
            if ( null != switchChars )
            {
                Debug.Assert ( switchChars.Length > 0 ,
                               "switchChars.Length > 0" );
            }
#endif
            if ( ( null == switchChars ) || ( 0 == switchChars.Length ) )
            {
                throw new ArgumentException ( Constants.ArrayMustBeValid ,
                                              "switchChars" );
            }

            this.flagSymbols = flagSymbols;
            this.dataSymbols = dataSymbols;
            this.caseSensitiveSwitches = caseSensitiveSwitches;
            this.switchChars = switchChars;
        }

        /// <summary>
        /// Reports correct command line usage.
        /// </summary>
        /// <param name="errorInfo">
        /// The string with the invalid command line option.
        /// </param>
        public abstract void OnUsage ( String errorInfo );


        // Every derived class must implement an OnSwitch method or a switch is 
        // considered an error.
        /// <summary>
        /// Called when a switch is parsed out.
        /// </summary>
        /// <param name="switchSymbol">
        /// The switch value parsed out.
        /// </param>
        /// <param name="switchValue">
        /// The value of the switch.  For flag switches this is null/Nothing.
        /// </param>
        /// <returns>
        /// One of the <see cref="SwitchStatus"/> values.
        /// </returns>
        protected virtual SwitchStatus OnSwitch ( String switchSymbol ,
                                                  String switchValue )
        {
            return ( SwitchStatus.Error );
        }

        /// <summary>
        /// Called when a non-switch value is parsed out.
        /// </summary>
        /// <param name="value">
        /// The value parsed out.
        /// </param>
        /// <returns>
        /// One of the <see cref="SwitchStatus"/> values.
        /// </returns>
        protected virtual SwitchStatus OnNonSwitch ( String value )
        {
            return ( SwitchStatus.Error );
        }

        /// <summary>
        /// Called when parsing is finished so final sanity checking can be 
        /// performed.
        /// </summary>
        /// <returns>
        /// One of the <see cref="SwitchStatus"/> values.
        /// </returns>
        protected virtual SwitchStatus OnDoneParse ( )
        {
            // By default, we'll assume that all parsing was an error.
            return ( SwitchStatus.Error );
        }

        ///// <summary>
        ///// Parses the application's command-line arguments.
        ///// </summary>
        ///// <returns>
        ///// True if the parsing succeeded.
        ///// </returns>
        //public Boolean Parse ( )
        //{
        //    // Visual Basic will use this method since its entry-point function 
        //    // doesn't get the command-line arguments passed to it.
        //    return ( Parse ( Environment.GetCommandLineArgs ( ) ) );
        //}

        // Looks to see if the switch is in the array.
        private int IsSwitchInArray ( String [] switchArray ,
                                      String value )
        {
            String valueCompare = value;
            if ( true == caseSensitiveSwitches )
            {
                valueCompare = value.ToUpperInvariant ( );
            }
            int retValue = -1;
            for ( int n = 0 ; n < switchArray.Length ; n++ )
            {
                String currSwitch = switchArray [ n ];
                if ( true == caseSensitiveSwitches )
                {
                    currSwitch = currSwitch.ToUpperInvariant ( );
                }
                if ( 0 == String.CompareOrdinal ( valueCompare ,
                                                  currSwitch ) )
                {
                    retValue = n;
                    break;
                }
            }
            return ( retValue );
        }

        /// <summary>
        /// Looks to see if this string starts with a switch character.
        /// </summary>
        /// <param name="value">
        /// The string to check.
        /// </param>
        /// <returns>
        /// True if the string starts with a switch character.
        /// </returns>
        private Boolean StartsWithSwitchChar ( String value )
        {
            Boolean isSwitch = false;
            for ( int n = 0 ; !isSwitch && ( n < switchChars.Length ) ; n++ )
            {
                if ( 0 == String.CompareOrdinal ( value ,
                                                  0 ,
                                                  switchChars [ n ] ,
                                                  0 ,
                                                  1 ) )
                {
                    isSwitch = true;
                    break;
                }
            }
            return ( isSwitch );
        }

        /// <summary>
        /// Parses an arbitrary set of arguments.
        /// </summary>
        /// <param name="args">
        /// The string array to parse through.
        /// </param>
        /// <returns>
        /// True if parsing was correct.  
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="args"/> is null or empty.
        /// </exception>
        [SuppressMessage ( "Microsoft.Globalization" ,
                           "CA1308:NormalizeStringsToUppercase" ,
                           Justification = "I NEED TO FIX THIS!" )]
        public Boolean Parse ( String [] args )
        {
            Debug.Assert ( null != args , "null != args" );
            // Avoid side effects in debug builds.
            if ( null == args ) 
            {
                throw new ArgumentException ( Constants.InvalidParameter );
            }
            // If there are no arguments, leave now.
            if ( args.Length == 0 )
            {
                return ( false ) ;
            }
            // Assume parsing is successful.
            SwitchStatus ss = SwitchStatus.NoError;

            int errorArg = -1;
            int currArg;
            for ( currArg = 0 ;
                 ( ss == SwitchStatus.NoError ) && ( currArg < args.Length ) ;
                 currArg++ )
            {
                errorArg = currArg;
                // Determine if this argument starts with a valid switch 
                // character
                Boolean isSwitch = StartsWithSwitchChar ( args [ currArg ] );
                if ( true == isSwitch )
                {
                    // Indicates the symbol is a data symbol.
                    Boolean useDataSymbols = false;
                    // Get the argument itself.
                    String processedArg = args [ currArg ].Substring ( 1 );
                    // The index into the symbol array.
                    int n;
                    // First check the flags array.
                    n = IsSwitchInArray ( flagSymbols , processedArg );
                    // If it's not in the flags array, try the data array if that 
                    // array is not null.
                    if ( ( -1 == n ) && ( null != dataSymbols ) )
                    {
                        n = IsSwitchInArray ( dataSymbols , processedArg );
                        useDataSymbols = true;
                    }
                    if ( -1 != n )
                    {
                        String theSwitch = null;
                        String dataValue = null;
                        // If it's a flag switch.
                        if ( false == useDataSymbols )
                        {
                            // This is a legal switch, notified the derived 
                            // class of this switch and its value.
                            theSwitch = flagSymbols [ n ];
                            if ( caseSensitiveSwitches )
                            {
                                theSwitch = flagSymbols [ n ].
                                                           ToLowerInvariant ( );
                            }
                        }
                        else
                        {
                            theSwitch = dataSymbols [ n ];
                            // Look at the next parameter if it's there.
                            if ( currArg + 1 < args.Length )
                            {
                                currArg++;
                                dataValue = args [ currArg ];
                                // Take a look at dataValue to see if it starts
                                // with a switch character. If it does, that
                                // means this data argument is empty.
                                if ( true == StartsWithSwitchChar ( 
                                                                   dataValue ) )
                                {
                                    ss = SwitchStatus.Error;
                                    break;
                                }
                            }
                            else
                            {
                                ss = SwitchStatus.Error;
                                break;
                            }
                        }
                        ss = OnSwitch ( theSwitch , dataValue );
                    }
                    else
                    {
                        ss = SwitchStatus.Error;
                        break;
                    }
                }
                else
                {
                    // This is not a switch, notified the derived class of this 
                    // "non-switch value"
                    ss = OnNonSwitch ( args [ currArg ] );
                }
            }

            // Finished parsing arguments
            if ( ss == SwitchStatus.NoError )
            {
                // No error occurred while parsing, let derived class perform a 
                // sanity check and return an appropriate status
                ss = OnDoneParse ( );
            }

            if ( ss == SwitchStatus.ShowUsage )
            {
                // Status indicates that usage should be shown, show it
                OnUsage ( null );
            }

            if ( ss == SwitchStatus.Error )
            {
                String errorValue = null;
                if ( ( errorArg != -1 ) && ( errorArg != args.Length ) )
                {
                    errorValue = args [ errorArg ];
                }
                // Status indicates that an error occurred, show it and the 
                // proper usage
                OnUsage ( errorValue );
            }

            // Return whether all parsing was successful.
            return ( ss == SwitchStatus.NoError );
        }
    }
}