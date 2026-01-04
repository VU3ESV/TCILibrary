// Copyright (c) 2019 Vinod ES (VU3ESV)
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

namespace ExpertElectronics.Tci.Events;

public class SpotEventArgs(string callSign, string mode, long frequencyInHz, Color color, string additionalText) : EventArgs
{
    public string CallSign { get; } = callSign;

    public string Mode { get; } = mode;

    public long FrequencyInHz { get; } = frequencyInHz;

    public Color Color { get; } = color;

    public string AdditionalText { get; } = additionalText;
}