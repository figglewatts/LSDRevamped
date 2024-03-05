videoClip = GetEntity("FerrisWheelVideo").VideoClip

function start()
    this.PlayAnimation(0)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(9, 9)
    videoClip.Play(Unity.ColorRGB(1, 0, 0))
end