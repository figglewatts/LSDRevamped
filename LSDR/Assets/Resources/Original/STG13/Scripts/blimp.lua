require "dreams"

interacted = false
liftsOff = false
linked = false
flightTime = 0
moveSpeed = 0.3

function start()
    this.SetChildVisible(false)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function update()
    if not interacted then return end

    local before = flightTime
    flightTime = flightTime + Unity.DeltaTime()

    -- now and then it stretches the world a long way
    if crossed(before, 5) or crossed(before, 40) then
        if Random.OneIn(5) then
            DreamSystem.StretchDream(5, 12)
        end
    end

    if crossed(before, 35) then
        liftsOff = Random.OneIn(3)
    end

    this.MoveInDirection(this.Forward, moveSpeed)

    if liftsOff then
        this.MoveInDirection(this.Up, moveSpeed)
        this.Rotate(Unity.Vector3(0, -5, 0))
        if flightTime > 46 and not linked then
            linked = true
            link()
        end
    elseif flightTime > 135 then
        this.GameObject.SetActive(false)
    end
end

function crossed(before, seconds)
    return before < seconds and flightTime >= seconds
end

function interact()
    this.LogGraphContribution(-6, 7)

    if interacted then return end
    this.SetChildVisible(true)
    interacted = true
end

function link()
    DreamSystem.SetNextTransitionDream(dreams.ClockworkMachines)
    DreamSystem.TransitionToDream()
end