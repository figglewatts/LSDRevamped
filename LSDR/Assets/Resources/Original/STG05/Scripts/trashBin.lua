videoClip = GetEntity("FlyingVideoClip").VideoClip

function interact()
    this.LogGraphContribution(0, -7)
    
    if Random.OneIn(1) then
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    end

    this.Action
        .Do(|| this.PlayAnimation(0))
        .ThenWaitUntil(this.WaitForAnimation(0))
        .Then(|| this.StopAnimation())
        .ThenFinish()
end