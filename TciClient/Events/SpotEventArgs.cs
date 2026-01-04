// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class SpotEventArgs : EventArgs
{
    public SpotEventArgs(string callSign, string mode, long frequencyInHz, Color color, string additionalText)
    {
        CallSign = callSign;
        Mode = mode;
        FrequencyInHz = frequencyInHz;
        Color = color;
        AdditionalText = additionalText;
    }

    public string CallSign { get; }

    public string Mode { get; }

    public long FrequencyInHz { get; }

    public Color Color { get; }

    public string AdditionalText { get; }
}