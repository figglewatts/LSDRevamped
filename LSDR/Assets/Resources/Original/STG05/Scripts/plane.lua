audio = GetEntity("PlaneAudio").DreamAudio
videoClip = GetEntity("PlaneVideoClip").VideoClip

state = "none"
moveSpeed = 5

function start()
    if Random.OneIn(3) then
        state = "flyover"
    else
        state = "crash"
    end
    
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.SetChildVisible(true)
    this.LogGraphContribution(-9, -3)
    audio.Play()

    if state == "crash" then
        this.Action
            .Do(|| this.PlayAnimation(0))
            .ThenWaitUntil(this.WaitForAnimation(0))
            .Then(crashed)
            .ThenFinish()
    else

        this.Action
            .Do(|| flyArcDown())
            .Until(Condition.WaitForSeconds(5))
            .Then(|| flyArcUp())
            .Until(Condition.WaitForSeconds(15))
            .Then(|| this.GameObject.SetActive(false))
            .ThenFinish()
    end
end

function flyArcDown()
    this.MoveInDirection(this.Forward, moveSpeed)
    this.MoveInDirection(this.Up.negated(), 0.6)
end

function flyArcUp()
    this.MoveInDirection(this.Forward, moveSpeed)
    this.MoveInDirection(this.Up, 1)
end

function crashed()
    this.GameObject.SetActive(false)
    if Random.OneIn(2) then
        videoClip.Play(Unity.ColorRGB(1, 0, 0))
    end
end