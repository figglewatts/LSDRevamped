audio = GetEntity(this.GameObject.Name .. "Audio").DreamAudio

moveSpeed = 0.15

function start()
    if IsDayEven() or Random.OneIn(2) then
        this.GameObject.SetActive(false)
        return
    end
    
    this.PlayAnimation(0)
    audio.Play()
end

function update()
    this.MoveInDirection(this.GameObject.ForwardDirection.negated(), moveSpeed)
end

function interact()
    this.LogGraphContribution(0, 1)
end