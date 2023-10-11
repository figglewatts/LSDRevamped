a = this.GameObject.GetChildByName("UnderwaterDudeOnWaterA").AnimatedObject
b = this.GameObject.GetChildByName("UnderwaterDudeOnWaterB").AnimatedObject

function start()
    if IsDayEven() or not Random.OneIn(4) then
        this.GameObject.SetActive(false)
        return
    end

    a.Play(0)
    b.Play(0)
end