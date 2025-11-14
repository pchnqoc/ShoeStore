// Auto-play carousel
document.addEventListener('DOMContentLoaded', function() {
    const carousel = document.querySelector('#productCarousel');
    if (carousel) {
        const carouselInstance = new bootstrap.Carousel(carousel, {
            interval: 5000,
            wrap: true,
            pause: 'hover'
        });
    }

    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Navbar auth guard & scroll effect
    let lastScroll = 0;
    const navbar = document.querySelector('.navbar');
    const cartLinks = document.querySelectorAll('[data-requires-auth="true"]');
    cartLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetUrl = this.dataset.targetUrl || '/Cart';
            fetch('/Account/CheckAuth', {
                method: 'GET',
                headers: {
                    'Accept': 'application/json'
                }
            })
            .then(r => r.ok ? r.json() : r.json().then(err => Promise.reject(err)))
            .then(data => {
                if (data.isAuthenticated) {
                    window.location.href = targetUrl;
                } else {
                    showNotification('Vui lòng đăng nhập để xem giỏ hàng.', 'warning');
                }
            })
            .catch(error => {
                console.error('Auth check error:', error);
                showNotification('Vui lòng đăng nhập để sử dụng chức năng giỏ hàng.', 'warning');
            });
        });
    });

    if (navbar) {
        window.addEventListener('scroll', function() {
            const currentScroll = window.pageYOffset;
            if (currentScroll > 100) {
                navbar.classList.add('shadow-lg');
            } else {
                navbar.classList.remove('shadow-lg');
            }
            lastScroll = currentScroll;
        });
    }

    // Product card animation on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    document.querySelectorAll('.product-item').forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(20px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });
});

// Filter functionality - called when filter dropdowns change
function applyFilter() {
    const danhMuc = document.getElementById('filterDanhMuc')?.value || '';
    const thuongHieu = document.getElementById('filterThuongHieu')?.value || '';
    const kichCo = document.getElementById('filterKichCo')?.value || '';
    const mauSac = document.getElementById('filterMauSac')?.value || '';

    // Show loading
    const productGrid = document.getElementById('productGrid');
    if (productGrid) {
        productGrid.innerHTML = '<div class="col-12 text-center py-5"><div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div></div>';
    }

    // Make AJAX call to filter products
    fetch('/Home/FilterProducts', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            DanhMuc: danhMuc ? parseInt(danhMuc) : null,
            ThuongHieu: thuongHieu ? parseInt(thuongHieu) : null,
            KichCo: kichCo ? parseInt(kichCo) : null,
            MauSac: mauSac ? parseInt(mauSac) : null
        })
    })
    .then(response => response.text())
    .then(html => {
        if (productGrid) {
            productGrid.innerHTML = html;
            // Re-initialize product animations
            initializeProductAnimations();
        }
    })
    .catch(error => {
        console.error('Error filtering products:', error);
        showNotification('Có lỗi xảy ra khi lọc sản phẩm', 'danger');
    });
}

function initializeProductAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    document.querySelectorAll('.product-item').forEach(item => {
        item.style.opacity = '0';
        item.style.transform = 'translateY(20px)';
        item.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(item);
    });
}

// Reset filter
function resetFilter() {
    document.getElementById('filterDanhMuc').value = '';
    document.getElementById('filterThuongHieu').value = '';
    document.getElementById('filterKichCo').value = '';
    document.getElementById('filterMauSac').value = '';
    
        // Reset to show all products
        location.reload();
}

// Add to cart function
function themVaoGioHang(maSanPham, options = {}) {
    // Check authentication first
    fetch('/Account/CheckAuth')
        .then(response => response.json())
        .then(authData => {
            if (!authData.isAuthenticated) {
                showNotification('Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng', 'warning');
                return;
            }

            const btn = event ? event.target.closest('button') : null;
            const originalText = btn ? btn.innerHTML : null;
            if (btn) {
                btn.innerHTML = '<span class="loading"></span> Đang thêm...';
                btn.disabled = true;
            }

            const payload = {
                productId: maSanPham,
                quantity: options.quantity || 1,
                bienTheSpId: options.bienTheSpId || null,
                size: options.size || null,
                color: options.color || null
            };

            return fetch('/Cart/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(async response => {
                const data = await response.json();
                if (!response.ok) {
                    return Promise.reject(data);
                }
                return data;
            })
            .then(data => {
                updateCartBadge(data.totalQuantity ?? 0);
                showNotification(data.message || 'Đã thêm sản phẩm vào giỏ hàng!', 'success');
            })
            .catch(error => {
                if (error) {
                    console.error('Add to cart error:', error);
                    showNotification(error.message || 'Không thể thêm sản phẩm vào giỏ hàng', 'danger');
                }
            })
            .finally(() => {
                if (btn) {
                    btn.innerHTML = originalText;
                    btn.disabled = false;
                }
            });
        })
        .catch(error => {
            console.error('Auth check error:', error);
            showNotification('Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng', 'warning');
        });
}

// View product details
function xemChiTiet(maSanPham) {
    window.location.href = `/SanPham/ChiTiet/${maSanPham}`;
}

// Update cart count (should be called after adding to cart)
function updateCartCount() {
    const cartBadge = document.querySelector('.cart-count-badge');
    if (!cartBadge) return;
    const currentCount = parseInt(cartBadge.textContent) || 0;
    updateCartBadge(currentCount + 1);
}

function updateCartBadge(count) {
    const cartLink = document.querySelector('.cart-link');
    if (!cartLink) return;

    let badge = cartLink.querySelector('.cart-count-badge');
    if (count > 0) {
        if (!badge) {
            badge = document.createElement('span');
            badge.className = 'position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger cart-count-badge';
            cartLink.appendChild(badge);
        }
        badge.textContent = count;
    } else if (badge) {
        badge.remove();
    }
}

function quickAddToCart(button) {
    if (!button) return;
    const productId = parseInt(button.dataset.productId);
    const variantId = button.dataset.bienthe ? parseInt(button.dataset.bienthe) : null;
    const size = button.dataset.size || null;
    const color = button.dataset.color || null;

    if (!productId) {
        console.warn('Missing product id for quick add.');
        return;
    }

    // Check authentication first
    fetch('/Account/CheckAuth')
        .then(response => response.json())
        .then(authData => {
            if (!authData.isAuthenticated) {
                showNotification('Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng', 'warning');
                return;
            }

            const originalText = button.innerHTML;
            button.innerHTML = '<span class="loading"></span> Đang thêm...';
            button.disabled = true;

            const payload = {
                productId: productId,
                quantity: 1,
                bienTheSpId: variantId,
                size: size,
                color: color
            };

            return fetch('/Cart/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload)
            })
            .then(async response => {
                const data = await response.json();
                if (!response.ok) {
                    return Promise.reject(data);
                }
                return data;
            })
            .then(data => {
                updateCartBadge(data.totalQuantity ?? 0);
                showNotification(data.message || 'Đã thêm sản phẩm vào giỏ hàng!', 'success');
            })
            .catch(error => {
                if (error) {
                    console.error('Add to cart error:', error);
                    showNotification(error.message || 'Không thể thêm sản phẩm vào giỏ hàng', 'danger');
                }
            })
            .finally(() => {
                button.innerHTML = originalText;
                button.disabled = false;
            });
        })
        .catch(error => {
            console.error('Auth check error:', error);
            showNotification('Bạn phải đăng nhập để thêm sản phẩm vào giỏ hàng', 'warning');
        });
}

// Show notification
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'success' ? 'success' : 'info'} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// Logout function
function dangXuat() {
    if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
        // Make API call to logout
        fetch('/Account/Logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
        .then(response => {
            if (response.ok) {
                window.location.href = '/Home/Index';
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('Có lỗi xảy ra khi đăng xuất', 'danger');
        });
    }
}

// Lazy load images
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                if (img.dataset.src) {
                    img.src = img.dataset.src;
                    img.removeAttribute('data-src');
                    observer.unobserve(img);
                }
            }
        });
    });

    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}

// Search functionality (if needed)
function searchProducts(query) {
    // Implement search functionality
    console.log('Searching for:', query);
}

// Price format helper
function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(price);
}

// Rating stars helper
function renderStars(rating) {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    let stars = '';
    
    for (let i = 0; i < fullStars; i++) {
        stars += '<i class="bi bi-star-fill text-warning"></i>';
    }
    
    if (hasHalfStar) {
        stars += '<i class="bi bi-star-half text-warning"></i>';
    }
    
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);
    for (let i = 0; i < emptyStars; i++) {
        stars += '<i class="bi bi-star text-muted"></i>';
    }
    
    return stars;
}
