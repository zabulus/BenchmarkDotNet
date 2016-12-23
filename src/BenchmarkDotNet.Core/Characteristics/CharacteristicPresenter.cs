﻿using System;
using BenchmarkDotNet.Core.Helpers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Helpers;

namespace BenchmarkDotNet.Characteristics
{
    public abstract class CharacteristicPresenter
    {
        public static readonly CharacteristicPresenter DefaultPresenter = new DefaultCharacteristicPresenter();
        public static readonly CharacteristicPresenter FolderPresenter = new FolderCharacteristicPresenter();
        public static readonly CharacteristicPresenter SummaryPresenter = new DefaultCharacteristicPresenter();
        public static readonly CharacteristicPresenter SourceCodePresenter = new SourceCodeCharacteristicPresenter();

        public abstract string ToPresentation(JobMode jobMode, Characteristic characteristic);

        private class DefaultCharacteristicPresenter : CharacteristicPresenter
        {
            public override string ToPresentation(JobMode jobMode, Characteristic characteristic)
            {
                if (!jobMode.HasValue(characteristic))
                    return "Default";

                var value = characteristic[jobMode];
                return (value as IFormattable)?.ToString(null, HostEnvironmentInfo.MainCultureInfo)
                    ?? value?.ToString() 
                    ?? "";
            }
        }

        private class SourceCodeCharacteristicPresenter : CharacteristicPresenter
        {
            public override string ToPresentation(JobMode jobMode, Characteristic characteristic)
            {
                // TODO: DO NOT hardcode Characteristic suffix
                var id = characteristic.Id;
                var type = characteristic.DeclaringType.FullName;
                var value = SourceCodeHelper.ToSourceCode(characteristic[jobMode]);
                if (value == string.Empty)
                {
                    return string.Empty;
                }
                else
                {
                    return $"{type}.{id}Characteristic[job] = {value}";
                }
            }
        }

        private class FolderCharacteristicPresenter : CharacteristicPresenter
        {
            public override string ToPresentation(JobMode jobMode, Characteristic characteristic)
            {
                return jobMode.HasValue(characteristic)
                    ? FolderNameHelper.ToFolderName(characteristic[jobMode])
                    : "Default";
            }
        }
    }
}