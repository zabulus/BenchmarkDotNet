﻿using System;
using System.Diagnostics;
using BenchmarkDotNet.Loggers;
using JetBrains.Annotations;

namespace BenchmarkDotNet.Extensions
{
    // we need it public to reuse it in the auto-generated dll
    // but we hide it from intellisense with following attribute
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class ProcessExtensions
    {
        public static void EnsureHighPriority(this Process process, ILogger logger)
        {
            try
            {
                process.PriorityClass = ProcessPriorityClass.High;
            }
            catch (Exception ex)
            {
                logger.WriteLineError($"Failed to set up high priority. Make sure you have the right permissions. Message: {ex.Message}");
            }
        }

        private static IntPtr FixAffinity(IntPtr processorAffinity)
        {
            int cpuMask = (1 << Environment.ProcessorCount) - 1;

            return IntPtr.Size == sizeof(Int64) 
                ? new IntPtr(processorAffinity.ToInt64() & cpuMask)
                : new IntPtr(processorAffinity.ToInt32() & cpuMask);
        }

        public static void EnsureProcessorAffinity(this Process process, IntPtr value)
        {
            process.ProcessorAffinity = FixAffinity(value);
        }

        public static bool TrySetPriority(
            [NotNull] this Process process,
            ProcessPriorityClass priority,
            [NotNull] ILogger logger)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            try
            {
                process.PriorityClass = priority;
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLineError(
                    $"// ! Failed to set up priority {priority} for process {process}. Make sure you have the right permissions. Message: {ex.Message}");
            }

            return false;
        }

        public static bool TrySetAffinity(
            [NotNull] this Process process,
            IntPtr processorAffinity,
            [NotNull] ILogger logger)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            try
            {
                process.ProcessorAffinity = FixAffinity(processorAffinity);
                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLineError(
                    $"// ! Failed to set up processor affinity 0x{(long)processorAffinity:X} for process {process}. Make sure you have the right permissions. Message: {ex.Message}");
            }

            return false;
        }
    }
}