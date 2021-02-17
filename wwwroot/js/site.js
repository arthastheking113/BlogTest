function Create_Success() {
    Swal.fire({
        position: 'center',
        icon: 'success',
        title: 'Create Success!!!',
        showConfirmButton: false,
        timer: 1500
    })
}
function Delete_Success() {
    Swal.fire({
        position: 'center',
        icon: 'success',
        title: 'Create Success!!!',
        showConfirmButton: false,
        timer: 1500
    })
}
document.getElementById("dropdownMenuLink").addEventListener("click", function () {
    if (screen.width >= 992) {
        $('.dropdown-item').addClass('text-white')
    }
    else
    {
        $('.dropdown-item').removeClass('text-white')
        $('.dropdown-item').addClass('text-dark')
    }
})