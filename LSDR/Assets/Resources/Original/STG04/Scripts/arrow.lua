function start()
    this.PlayAnimation(0)
    
    if IsDayEven() or (this.GameObject.Name == "QuestionMark2" and ChartZone() ~= "black") then
        this.GameObject.SetActive(false)
        return
    end
end